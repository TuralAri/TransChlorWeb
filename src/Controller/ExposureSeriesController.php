<?php

namespace App\Controller;

use App\Entity\Exposure;
use App\Entity\ExposureSeries;
use App\Entity\WeatherStation;
use App\Form\ExposureSeriesFormType;
use App\Repository\ExposureSeriesRepository;
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
        $exposureSeries->setLabel('test');
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
                $response = $this->calculate($formDataArray,$meteoFileName);
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
                }

                //Envoi du formulaire ainsi que du MeteoFile vers l'API C#
                //ON ATTENDRA UN FICHIER ZIP
                $response = $this->generateExpositions($formDataArray, $meteoFileName);

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

    public function sendFileForCalc(String $meteoFileName ,String $fileName, String $route): Response
    {
//        $filePath1 = $this->getParameter('kernel.project_dir') . '/public/meteoFiles/' . $meteoFileName;
        $uploadDirectory = $this->getParameter('upload_directory') . '/Ressources';

        //COPIE DU FICHIER METEO POUR L ENVOYER SOUS UN NOM UNIQUE (ENTRE UTILISATEURS)
        $originalMeteoFilePath = $uploadDirectory . '/MeteoFiles/' . $meteoFileName;
        $tempMeteoFileName = uniqid('temp_') . '_' . $meteoFileName;
        $tempMeteoFilePath = $uploadDirectory . '/MeteoFiles/' . $tempMeteoFileName;
        copy($originalMeteoFilePath, $tempMeteoFilePath);

        $file1 = fopen($tempMeteoFilePath, 'r'); //on ouvre le fichier meteo

        $filePath2 = $uploadDirectory . '/out/' . $fileName;
        $file2 = fopen($filePath2, 'r');
        $client = HttpClient::create();
        $response = $client->request('POST', 'http://localhost:5000/' . $route, [
            'headers' => [
                'Content-Type' => 'multipart/form-data'
            ],
            'body' => [
                'file1' => $file1,
                'file2' => $file2
            ]
        ]);

        //NETTOYAGE
        unlink($tempMeteoFilePath);

        return new Response($response->getContent(), $response->getStatusCode());
    }

    public function extractCalcValues(string $filePath): array
    {
        $lines = file($filePath, FILE_IGNORE_NEW_LINES | FILE_SKIP_EMPTY_LINES);

        if (count($lines) < 32) {
            throw new \Exception('Le fichier précalcul est incomplet.');
        }

        return [
            'mechanicalInterventions' => floatval(str_replace(',', '.', $lines[6])),
            'automaticSprays' => floatval(str_replace(',', '.', $lines[12])),
            'mechanicalThresholdTemperature' => floatval(str_replace(',', '.', $lines[9])),
            'automaticThresholdTemperature' => floatval(str_replace(',', '.', $lines[15])),
        ];
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

    public function initAfterCalc(String $filePath): JsonResponse
    {
        if (!file_exists($filePath)) {
            return new JsonResponse(['error' => 'Fichier non trouvé'], Response::HTTP_NOT_FOUND);
        }

        $lines = file($filePath, FILE_IGNORE_NEW_LINES | FILE_SKIP_EMPTY_LINES);
        if (count($lines) < 36) {
            return new JsonResponse(['error' => 'Fichier incomplet'], Response::HTTP_BAD_REQUEST);
        }

        $data = $this->extractCalcValues($filePath);

        return new JsonResponse($data);
    }

    public function calculate(array $data , String $meteoFileName ): JsonResponse
    {
        $uploadDirectory = $this->getParameter('upload_directory') . '/Ressources/';
        $uniqueId = uniqid();

        $outputFileName = 'form_meteo_input_' . $uniqueId . '.txt';
        $outputFilePath = $uploadDirectory . 'out/' . $outputFileName;

        $dataString = implode("\n", $data);
        file_put_contents($outputFilePath, $dataString);

        $response = $this->sendFileForCalc($meteoFileName, $outputFileName, 'calcul');
        if ($response->getStatusCode() === 200) {
            $responseContent = $response->getContent();
            $calcOutputFileName = 'calc_form_meteo_output_' . $uniqueId . '.txt';
            $calcOutputFilePath = $uploadDirectory . 'out/' . $calcOutputFileName;
            file_put_contents($calcOutputFilePath, $responseContent);

            $response = $this->initAfterCalc($calcOutputFilePath);

            unlink($outputFilePath);
            unlink($calcOutputFilePath);

            if ($response->getStatusCode() === 200) {
                // ✅ Retourne les données déjà en tableau
                return $response;
            } else {
                return new JsonResponse([
                    'error' => 'Erreur lors de l\'initialisation après calcul: ' . $response->getContent()
                ], 500);
            }

        } else {
            return new JsonResponse([
                'error' => 'Erreur lors de l\'envoi du fichier: ' . $response->getContent()
            ], 500);
        }
    }

    public function generateExpositions(array $data, String $meteoFileName): Response {
        $uploadDirectory = $this->getParameter('upload_directory') . '/Ressources/';
        $uniqueId = uniqid();

        $outputFileName = 'form_meteo_input_' . $uniqueId . '.txt';
        $outputFilePath = $uploadDirectory . 'out/' . $outputFileName;

        $data = array_map(function($value) {
            return is_float($value) || is_numeric($value) ? str_replace('.', ',', (string)$value) : $value;
        }, $data);

        $dataString = implode("\n", $data);
        file_put_contents($outputFilePath, $dataString);

        $response = $this->sendFileForCalc($meteoFileName, $outputFileName, 'export');

        unlink($outputFilePath);

        return $response;
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