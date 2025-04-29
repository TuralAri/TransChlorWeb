<?php

namespace App\Controller;

use App\Entity\WeatherStation;
use App\Form\WeatherStationFormType;
use App\Repository\WeatherStationRepository;
use App\Service\ApiService;
use DateTime;
use Doctrine\ORM\EntityManager;
use Doctrine\ORM\EntityManagerInterface;
use Symfony\Bundle\FrameworkBundle\Controller\AbstractController;
use Symfony\Component\HttpClient\HttpClient;
use Symfony\Component\HttpFoundation\File\Exception\FileException;
use Symfony\Component\HttpFoundation\File\UploadedFile;
use Symfony\Component\HttpFoundation\JsonResponse;
use Symfony\Component\HttpFoundation\Request;
use Symfony\Component\HttpFoundation\Response;
use Symfony\Component\Routing\Annotation\Route;
use function Symfony\Component\Translation\t;

class WeatherStationController extends AbstractController
{
    private $apiService;
    public function __construct(ApiService $apiService){
        $this->apiService = $apiService;
    }

    #[Route('/weatherstations', name: 'weather_stations')]
    public function index(WeatherStationRepository $repository)
    {
        if(!$this->getUser()){
            return $this->redirectToRoute('index');
        }

        $weatherStations = $this->getUser()->getMeteoFiles();

        return $this->render('weatherstations/index.html.twig', [
            'weatherStations' => $weatherStations,
        ]);
    }

    #[Route('/weatherstations/add', name: 'weather_station_add')]
    public function add(EntityManagerInterface $em, Request $request)
    {
        if(!$this->getUser()){
            return $this->redirectToRoute('index');
        }

        $weatherStation = new WeatherStation();
        $form = $this->createForm(WeatherStationFormType::class, $weatherStation);
        $form->handleRequest($request);

        if($form->isSubmitted() && $form->isValid() && $request->getMethod() == 'POST'){
            $uploadMeteoFile = $form->get('filename')->getData();

            $weatherStation = $form->getData();
            $weatherStation->setLocalFileName($uploadMeteoFile->getClientOriginalName());
            $weatherStation->setUploadedBy($this->getUser());
            $weatherStation->setUploadedAt(new \DateTimeImmutable('now'));

            $newFileName = time() . '.' . $uploadMeteoFile->guessExtension();

            $uploadDirectory = $this->getParameter('upload_directory') . '/Ressources/MeteoFiles';

            try{
                $uploadReponse = $this->apiService->upload($uploadMeteoFile, $newFileName);
                if($uploadReponse->getStatusCode() === 200){
                    $filePath = $uploadReponse->getContent();

                    $troubleshootResults = $this->apiService->runTroubleshoots($filePath);
                    foreach ($troubleshootResults as $result) {
                        $this->addFlash($result['type'], $result['message']);
                    }

                    $precalculateResponse = $this->apiService->precalculate($filePath, $newFileName);

                    if($precalculateResponse->getStatusCode() === 200){
                        $responseContent = $precalculateResponse->getContent();
                        $values = json_decode($responseContent, true);

                        $meteoFileVars = $this->readMeteoFileVars($uploadDirectory . '/' . $newFileName);
                        $weatherStation->setMeteoFileVars($meteoFileVars);

                        if (isset($values[0]) && is_string($values[0])) {
                            $values = json_decode($values[0], true);
                        }

                        $weatherStation->setFilename($newFileName);
                        $weatherStation->setFileYears($values['fileYears']);
                        $weatherStation->setMechanicalAnnualSodium($values['mechanicalAnnualSodium']);
                        $weatherStation->setAutomaticAnnualSodium($values['automaticAnnualSodium']);

                        $em->persist($weatherStation);
                        $em->flush();
                        $this->addFlash('success', 'Fichier importé avec succès');
                    }else{
                        $this->addFlash('error', 'Erreur lors de l\'importation du fichier');
                    }
                }
            }catch (FileException $e){
                $this->addFlash('error', 'Erreur lors de l\'upload du fichier');
            }

            return $this->redirectToRoute('weather_stations');
        }

        return $this->render('weatherstations/add.html.twig', [
            'form' => $form->createView(),
        ]);
    }

    /**
     * @param string $filePath
     * @return array
     * reads the first 2 lines from MeteoFile in order to extract
     * and return the following values [stationNumber, stationName, startDate, endDate]
     */
    public function readMeteoFileVars(string $filePath): array
    {
        $handle = fopen($filePath, 'r');
        if($handle){
            $lines = [];
            for($i = 0; $i < 2; $i++){
                if(($line = fgets($handle)) !== false){
                    $lines[] = trim($line); //Appends next line to the $lines array
                }
            }
        }

        fclose($handle);

        if(count($lines) >=1){
            preg_match('/(\d+)\s*-\s*(.+)/', $lines[0], $matchesStation);
            $number = $matchesStation[1] ?? null;
            $name = $matchesStation[2] ?? null;
        }

        if(count($lines) >=2){
            preg_match('/(\d{1,2}\.\d{4})\s+au\s+(\d{1,2}\.\d{4})/', $lines[1], $matchesDates);
            $startDate = $matchesDates[1] ?? null;
            $endDate = $matchesDates[2] ?? null;
        }

        if($startDate){
            $startDate = $this->convertToDateTime($startDate);
        }

        if($endDate){
            $endDate = $this->convertToDateTime($endDate);
        }

        $values = [
            'stationNumber' => $number ?? null,
            'stationName' => $name ?? null,
            'startDate' => $startDate ?? null,
            'endDate' => $endDate ?? null,
        ];

        return $values;
    }

    /**
     * @param $dateString
     * @return DateTime
     * Receives a string "month.year" and returns
     * a datetime from it
     */
    private function convertToDateTime($dateString) : DateTime{
        $dateParts = explode('.', $dateString);
        $month = $dateParts[0];  // Mois
        $year = $dateParts[1];   // Année

        return new DateTime("{$year}-{$month}-01");
    }

}