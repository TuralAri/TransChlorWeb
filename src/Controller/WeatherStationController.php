<?php

namespace App\Controller;

use App\Entity\WeatherStation;
use App\Form\WeatherStationFormType;
use App\Repository\WeatherStationRepository;
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
    public function add(EntityManagerInterface $em, Request $request, WeatherStationRepository $repository)
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
                $jsonResponse = $this->uploadAndPrecalculate($uploadMeteoFile, $newFileName);
                if($jsonResponse->getStatusCode() === 200){
                    $responseContent = $jsonResponse->getContent();
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
            }catch (FileException $e){
                //On verra ça plus tard
                $this->addFlash('error', 'Erreur lors de l\'upload du fichier');
            }

            return $this->redirectToRoute('weather_stations');
        }

        return $this->render('weatherstations/add.html.twig', [
            'form' => $form->createView(),
        ]);
    }

    public function sendFile(String $fileName,String $route): Response
    {
        $filePath = $this->getParameter('upload_directory') . '/Ressources/MeteoFiles/' . $fileName;
        $file = fopen($filePath, 'r');
        $client = HttpClient::create();
        $response = $client->request('POST', 'http://localhost:5000/' . $route , [
            'headers' => [
                'Content-Type' => 'multipart/form-data'
            ],
            'body' => [
                'file' => $file
            ]
        ]);
        return new Response($response->getContent(), $response->getStatusCode());
    }

    public function troubleshoot(String $fileName) : Response{

        $response = $this->sendFile($fileName,'api/troubleshoot/troubleshoot1');

        if($response->getStatusCode() === 200) {
            $this->addFlash('success', $response->getContent());
        }

        $response = $this->sendFile($fileName,'api/troubleshoot/troubleshoot2');
        if($response->getStatusCode() === 200) {
            $this->addFlash('success', $response->getContent());
        }

        return new Response('Hello');
    }

    public function extractPrecalcValues(string $filePath): JsonResponse
    {
        $lines = file($filePath, FILE_IGNORE_NEW_LINES | FILE_SKIP_EMPTY_LINES);

        if (count($lines) < 32) {
            throw new \Exception('Le fichier précalcul est incomplet.');
        }

        $data = [
            'fileYears' => floatval(str_replace(',', '.', $lines[0])),
            'mechanicalAnnualSodium' => floatval(str_replace(',', '.', $lines[4])),
            'automaticAnnualSodium' => floatval(str_replace(',', '.', $lines[8])),
        ];

        return new JsonResponse($data);
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

    private function convertToDateTime($dateString) {
        $dateParts = explode('.', $dateString);
        $month = $dateParts[0];  // Mois
        $year = $dateParts[1];   // Année

        return new DateTime("{$year}-{$month}-01");
    }

    public function uploadAndPrecalculate(UploadedFile $file, string $newFileName): JsonResponse
    {
        $destination = $this->getParameter('upload_directory') . '/Ressources/MeteoFiles';

        try {
            $file->move($destination, $newFileName);
            $this->troubleshoot($newFileName);
            $response = $this->sendFile($newFileName, 'api/data/precalcul');
            if ($response->getStatusCode() === 200) {
                $responseContent = $response->getContent();
                $outputFileName = 'form_meteo_output_' . $newFileName;
                $outputFilePath = $this->getParameter('upload_directory') . '/Ressources/out/' . $outputFileName;
                file_put_contents($outputFilePath, $responseContent);
            } else {
                return new JsonResponse([
                    'error' => 'Erreur lors de l\'envoi du fichier: ' . $response->getContent()
                ], 500);
            }
            $response = $this->extractPrecalcValues($outputFilePath);
            if(file_exists($outputFilePath)){
                unlink($outputFilePath);
            }

            return new JsonResponse([$response->getContent()], $response->getStatusCode());

        } catch (\Exception $e) {
            error_log('Upload error' . $e->getMessage());

            return new JsonResponse([
                'error' => 'Erreur lors de l\'upload: ' . $e->getMessage()
            ], 500);
        }

    }

}