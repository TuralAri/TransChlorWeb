<?php

namespace App\Controller;

use App\Entity\Exposure;
use App\Entity\ExposureSeries;
use App\Entity\WeatherStation;
use App\Form\ExposureSeriesFormType;
use App\Repository\ExposureSeriesRepository;
use App\Service\ApiService;
use Doctrine\ORM\EntityManagerInterface;
use Symfony\Bundle\FrameworkBundle\Controller\AbstractController;
use Symfony\Component\HttpClient\HttpClient;
use Symfony\Component\HttpFoundation\JsonResponse;
use Symfony\Component\HttpFoundation\Request;
use Symfony\Component\HttpFoundation\Response;
use Symfony\Component\Routing\Annotation\Route;
use ZipArchive;

class ExposureSeriesController extends AbstractController
{
    private $apiService;
    public function __construct(ApiService $apiService){
        $this->apiService = $apiService;
    }

    #[Route('/weatherstations/{id}/exposure-series', name: 'exposure_series')]
    public function index(WeatherStation $weatherStation): Response
    {

        if(!$this->getUser()){
            return $this->redirectToRoute('index');
        }

        $exposureSeries = $weatherStation->getExposureSeries();

        return $this->render('exposure_series/index.html.twig', [
            'weatherStation' => $weatherStation,
            'exposureSeries' => $exposureSeries,
        ]);
    }
    #[Route('/weatherstations/{id}/exposure-series/generate', name: 'exposure_series_generate')]
    public function generate(WeatherStation $weatherStation, ExposureSeriesRepository $exposureSeriesRepository, Request $request, EntityManagerInterface $entityManager) : Response
    {
        if(!$this->getUser()){
            return $this->redirectToRoute('index');
        }

        $exposureSeries = new ExposureSeries();
        $exposureSeries->setWeatherStation($weatherStation);
        $exposureSeries->setFileYears($weatherStation->getFileYears());
        $exposureSeries->setMechanicalAnnualSodium($weatherStation->getMechanicalAnnualSodium());
        $exposureSeries->setAutomaticAnnualSodium($weatherStation->getAutomaticAnnualSodium());

        $exposureSeriesForm = $this->createForm(ExposureSeriesFormType::class, $exposureSeries);
        $exposureSeriesForm->handleRequest($request);

        if ($exposureSeriesForm->isSubmitted() && $exposureSeriesForm->isValid()) {

            //BOUTON CALCULER CLIC
            if ($exposureSeriesForm->get('submit')->isClicked()) {
                $exposureSeriesFormData = $exposureSeriesForm->getData();
                $formDataArray = $this->getFormData($exposureSeriesFormData, 0);
                $meteoFileName = $weatherStation->getFilename();

                //CALCUL DES 4 VALEURS MANQUANTES
                $response = $this->apiService->calculate($formDataArray,$meteoFileName);
                if($response->getStatusCode() == 200){
                    $responseContent = json_decode(trim($response->getContent()), true);

                    $exposureSeries->setMechanicalInterventions($responseContent['mechanicalInterventions']);
                    $exposureSeries->setAutomaticSprays($responseContent['automaticSprays']);
                    $exposureSeries->setMechanicalThresholdTemperature($responseContent['mechanicalThresholdTemperature']);
                    $exposureSeries->setAutomaticThresholdTemperature($responseContent['automaticThresholdTemperature']);

                    $exposureSeriesForm = $this->createForm(ExposureSeriesFormType::class, $exposureSeries);
                    $this->addFlash('success', 'Calcul effectué avec succès');
                }
            }

            // BOUTON GENERER CLIC
            elseif($exposureSeriesForm->get('generate')->isClicked()) {
                $exposureSeriesFormData = $exposureSeriesForm->getData();
                $formDataArray = $this->getFormData($exposureSeriesFormData, 2);
                $exposureSeries = $exposureSeriesForm->getData();
                $meteoFileName = $weatherStation->getFilename();

                if($exposureSeries->getMechanicalInterventions() === null || $exposureSeries->getMechanicalThresholdTemperature() == null
                    || $exposureSeries->getAutomaticThresholdTemperature() == null || $exposureSeries->getAutomaticSprays() == null){
                    $this->addFlash('error', 'Des champs ne sont pas remplis, avez vous essayé de calculer les valeurs manquantes ?');
                    return $this->redirectToRoute('exposure_series_generate', ['id' => $weatherStation->getId()]);
                }

                //Sending form and meteofile to C# API
                //We'll wait for a zip file
                $response = $this->apiService->generateExpositions($formDataArray, $meteoFileName);

                if($response->getStatusCode() == 200){
                    $uploadDirectory = $this->getParameter('upload_directory');
                    $zipContent = $response->getContent();

                    $entityManager->persist($exposureSeries);
                    $entityManager->flush();

                    $expositionDirectory = $uploadDirectory . '/Ressources/Exposition/' . $exposureSeries->getId();
                    if (!is_dir($expositionDirectory)) {
                        mkdir($expositionDirectory, 0775, true);
                    }

                    $zipTempPath = $expositionDirectory . uniqid() . 'temp.zip';
                    file_put_contents($zipTempPath, $zipContent);

                    $this->extractZip($zipTempPath,$expositionDirectory);
                    $this->processExposureFiles($expositionDirectory, $exposureSeries, $entityManager);
                    $this->addFlash('success','Les expositions ont bien été générées');
                    return $this->redirectToRoute('exposure_series', ['id' => $weatherStation->getId()]);
                }

            }

        }

        return $this->render('exposure_series/generate.html.twig', [
            'weatherStation' => $weatherStation,
            'exposureSeriesForm' => $exposureSeriesForm->createView(),
        ]);
    }

    public function getFormData($formData, int $case): array
    {
        if($case == 0){
            return  [
                'fileYears' => $formData->getFileYears(),
                'sodiumChlorideConcentration' => $formData->getSodiumChlorideConcentration(),
                'waterFilmThickness' => $formData->getWaterFilmThickness(),
                'humidityThreshold' => $formData->getHumidityThreshold(),
                'mechanicalAnnualSodium' => $formData->getMechanicalAnnualSodium(),
                'mechanicalMeanSodium' => $formData->getMechanicalMeanSodium(),
                'mechanicalInterval' => $formData->getMechanicalInterval(),
                'mechanicalSodiumWater' => $formData->getMechanicalSodiumWater(),
                'automaticAnnualSodium' => $formData->getAutomaticAnnualSodium(),
                'automaticMeanSodium' => $formData->getAutomaticMeanSodium(),
                'automaticSprayInterval' => $formData->getAutomaticSprayInterval(),
                'automaticSodiumWater' => $formData->getAutomaticSodiumWater(),
                'extTemperaturePosition' => $formData->getExtTemperaturePosition(),
                'extTemperaturePosition2' => $formData->getExtTemperaturePosition2(),
                'extTemperatureAttenuation' => $formData->getExtTemperatureAttenuation(),
                'extTemperatureAttenuation2' => $formData->getExtTemperatureAttenuation2(),
                'extTemperatureDifference' => $formData->getExtTemperatureDifference(),
                'extHumidityPosition' => $formData->getExtHumidityPosition(),
                'extHumidityPosition2' => $formData->getExtHumidityPosition2(),
                'extHumidityAttenuation' => $formData->getExtHumidityAttenuation(),
                'extHumidityAttenuation2' => $formData->getExtHumidityAttenuation2(),
                'extHumidityDifference' => $formData->getExtHumidityDifference(),
                'intTemperaturePosition' => $formData->getIntTemperaturePosition(),
                'intTemperaturePosition2' => $formData->getIntTemperaturePosition2(),
                'intTemperatureAttenuation' => $formData->getIntTemperatureAttenuation(),
                'intTemperatureAttenuation2' => $formData->getIntTemperatureAttenuation2(),
                'intTemperatureDifference' => $formData->getIntTemperatureDifference(),
                'intHumidityPosition' => $formData->getIntHumidityPosition(),
                'intHumidityPosition2' => $formData->getIntHumidityPosition2(),
                'intHumidityAttenuation' => $formData->getIntHumidityAttenuation(),
                'intHumidityAttenuation2' => $formData->getIntHumidityAttenuation2(),
                'intHumidityDifference' => $formData->getIntHumidityDifference()
            ];
        }
        elseif($case == 1){
            return [
            'mechanicalInterventions' => $formData->getMechanicalInterventions(),
            'automaticSprays' => $formData->getAutomaticSprays(),
            'mechanicalThresholdTemperature' => $formData->getMechanicalThresholdTemperature(),
            'automaticThresholdTemperature' => $formData->getAutomaticThresholdTemperature()
            ];
        }
        elseif($case == 2){
            return array_merge($this->getFormData($formData,0),$this->getFormData($formData,1));
        }
        return [];
    }

    public function extractZip($zipTempPath, $directory)
    {
        $zip = new ZipArchive();
        if($zip->open($zipTempPath)){
          $zip->extractTo($directory);
          $zip->close();
          unlink($zipTempPath);
          return true;
        }
        return false;
    }

    /**
     * @param string $folderPath Le dossier contenant les fichiers d'exposition
     * @param ExposureSeries $exposureSeries L'entité série d'expositions à laquelle lier les fichiers
     * @param EntityManagerInterface $entityManager Pour persister les entités
     */
    function processExposureFiles(string $folderPath, ExposureSeries $exposureSeries, EntityManagerInterface $entityManager): void
    {
        $files = scandir($folderPath);

        foreach ($files as $file) {
            if (is_file($folderPath . '/' . $file) && preg_match('/EXPO_([A-Z_]+)_temp.*\.txt/', $file, $matches)) {
                $type = $matches[1];
                $originalPath = $folderPath . '/' . $file;
                $newFilename = "EXPO_{$type}.txt";
                $newPath = $folderPath . '/' . $newFilename;

                rename($originalPath, $newPath);

                $exposure = new Exposure();
                $exposure->setExposureSerie($exposureSeries);
                $exposure->setType($type);
                $exposure->setFilename($newFilename);
                $exposure->setLocalname($file);

                $entityManager->persist($exposure);
            }
        }

        // Flush les exposures créées
        $entityManager->flush();
    }


}