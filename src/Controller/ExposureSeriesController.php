<?php

namespace App\Controller;

use App\Entity\ExposureSeries;
use App\Entity\WeatherStation;
use App\Form\ExposureSeriesFormType;
use App\Repository\ExposureSeriesRepository;
use Symfony\Bundle\FrameworkBundle\Controller\AbstractController;
use Symfony\Component\HttpClient\HttpClient;
use Symfony\Component\HttpFoundation\JsonResponse;
use Symfony\Component\HttpFoundation\Request;
use Symfony\Component\HttpFoundation\Response;
use Symfony\Component\Routing\Annotation\Route;

class ExposureSeriesController extends AbstractController
{
    #[Route('/weatherstations/{id}/exposure-series', name: 'exposure_series')]
    public function index(WeatherStation $weatherStation): Response
    {
        $exposureSeries = $weatherStation->getExposureSeries();

        return $this->render('exposure_series/index.html.twig', [
            'weatherStation' => $weatherStation,
            'exposureSeries' => $exposureSeries,
        ]);
    }
    #[Route('/weatherstations/{id}/exposure-series/generate', name: 'exposure_series_generate')]
    public function generate(WeatherStation $weatherStation, ExposureSeriesRepository $exposureSeriesRepository, Request $request) : Response
    {
        $exposureSeries = new ExposureSeries();
        $exposureSeries->setWeatherStation($weatherStation);
        $exposureSeries->setFileYears($weatherStation->getFileYears());
        $exposureSeries->setMechanicalAnnualSodium($weatherStation->getMechanicalAnnualSodium());
        $exposureSeries->setAutomaticAnnualSodium($weatherStation->getAutomaticAnnualSodium());

        $exposureSeriesForm = $this->createForm(ExposureSeriesFormType::class, $exposureSeries);
        $exposureSeriesForm->handleRequest($request);

        if ($exposureSeriesForm->isSubmitted() && $exposureSeriesForm->isValid()) {
            $exposureSeriesFormData = $exposureSeriesForm->getData();
            $formDataArray = $this->getFormData($exposureSeriesFormData);
            $meteoFileName = $weatherStation->getFilename();

            $response = $this->calculate($formDataArray,$meteoFileName);
            if($response->getStatusCode() == 200){
                $responseContent = json_decode(trim($response->getContent()), true);
                $exposureSeries->setMechanicalInterventions($responseContent['mechanicalInterventions']);
                $exposureSeries->setAutomaticSprays($responseContent['automaticSprays']);
                $exposureSeries->setMechanicalThresholdTemperature($responseContent['mechanicalThresholdTemperature']);
                $exposureSeries->setAutomaticThresholdTemperature($responseContent['automaticThresholdTemperature']);
                $exposureSeriesForm = $this->createForm(ExposureSeriesFormType::class, $exposureSeries);

                //remplir
                $this->addFlash('success', 'Calcul effectué avec succès');
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
        $filePath1 = $uploadDirectory . '/MeteoFiles/' . $meteoFileName;
        $file1 = fopen($filePath1, 'r');


//        $filePath2 = $this->getParameter('kernel.project_dir') . '/public/out/' . $fileName;
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


    public function getFormData($formData): array
    {
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


}