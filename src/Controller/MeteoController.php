<?php

namespace App\Controller;

use App\Entity\ImportFile;
use App\Entity\Meteo;
use App\Form\ImportFileType;
use App\Form\MeteoType;
use Symfony\Bundle\FrameworkBundle\Controller\AbstractController;
use Symfony\Component\HttpClient\HttpClient;
use Symfony\Component\HttpFoundation\JsonResponse;
use Symfony\Component\HttpFoundation\Request;
use Symfony\Component\HttpFoundation\Response;
use Symfony\Component\Routing\Annotation\Route;
use Symfony\Contracts\HttpClient\Exception\ClientExceptionInterface;
use Symfony\Contracts\HttpClient\Exception\RedirectionExceptionInterface;
use Symfony\Contracts\HttpClient\Exception\ServerExceptionInterface;
use Symfony\Contracts\HttpClient\Exception\TransportExceptionInterface;

class MeteoController extends AbstractController
{
    
    #[Route('/meteo', name: 'meteo_form')]
    public function index(Request $request): Response
    {
        $meteo = new Meteo();
        $form = $this->createForm(MeteoType::class, $meteo);
        $form->handleRequest($request);

        $importFile = new ImportFile();
        $importForm = $this->createForm(ImportFileType::class, $importFile);
        $importForm->handleRequest($request);


        if($importForm->isSubmitted() && $importForm->isValid()) {
            $this->uploadMeteo($request);
        }



        return $this->render('meteo/index.html.twig', [
            'form' => $form->createView(),
            'importForm' => $importForm->createView()
        ]);
    }



    public function uploadMeteo(Request $request): JsonResponse
    {
        $file = $request->files->get('import_file')['importFile'];
        if ($file) {
            $fileName = $file->getClientOriginalName();
            $destination = $this->getParameter('kernel.project_dir') . '/public/meteoFiles';


            try {
                $file->move($destination, $fileName);


                $response = $this->sendFile($fileName);
                if ($response->getStatusCode() === 200) {
                    $responseContent = $response->getContent();
                    $outputFileName = 'form_meteo_output.txt';
                    $outputFilePath = $this->getParameter('kernel.project_dir') . '/public/out/' . $outputFileName;
                    file_put_contents($outputFilePath, $responseContent);
                }else {
                    return new JsonResponse([
                        'error' => 'Erreur lors de l\'envoi du fichier: ' . $response->getContent()
                    ], 500);
                }

                $this->init($outputFilePath);

                return new JsonResponse([
                    'success' => true
                ]);
            } catch (\Exception $e) {
                error_log("Upload error: " . $e->getMessage());
                return new JsonResponse([
                    'error' => 'Erreur lors de l\'upload: ' . $e->getMessage()
                ], 500);
            }






        }

        return new JsonResponse(['error' => 'Aucun fichier reçu'], 400);
    }


    public function init(String $filePath): JsonResponse
    {

        $lines = file($filePath, FILE_IGNORE_NEW_LINES | FILE_SKIP_EMPTY_LINES);

        if (count($lines) < 32) {
            return new JsonResponse(['error' => 'Fichier incomplet'], Response::HTTP_BAD_REQUEST);
        }

        $data = [
            'fileYears' => $lines[0],
            'sodiumChlorideConcentration' => $lines[1],
            'waterFilmThickness' => $lines[2],
            'humidityThreshold' => $lines[3],
            'mechanicalAnnualSodium' => $lines[4],
            'mechanicalMeanSodium' => $lines[5],
            'mechanicalInterval' => $lines[6],
            'mechanicalSodiumWater' => $lines[7],
            'automaticAnnualSodium' => $lines[8],
            'automaticMeanSodium' => $lines[9],
            'automaticSprayInterval' => $lines[10],
            'automaticSodiumWater' => $lines[11],
            'extTemperaturePosition' => $lines[12],
            'extTemperaturePosition2' => $lines[13],
            'extTemperatureAttenuation' => $lines[14],
            'extTemperatureAttenuation2' => $lines[15],
            'extTemperatureDifference' => $lines[16],
            'extHumidityPosition' => $lines[17],
            'extHumidityPosition2' => $lines[18],
            'extHumidityAttenuation' => $lines[19],
            'extHumidityAttenuation2' => $lines[20],
            'extHumidityDifference' => $lines[21],
            'intTemperaturePosition' => $lines[22],
            'intTemperaturePosition2' => $lines[23],
            'intTemperatureAttenuation' => $lines[24],
            'intTemperatureAttenuation2' => $lines[25],
            'intTemperatureDifference' => $lines[26],
            'intHumidityPosition' => $lines[27],
            'intHumidityPosition2' => $lines[28],
            'intHumidityAttenuation' => $lines[29],
            'intHumidityAttenuation2' => $lines[30],
            'intHumidityDifference' => $lines[31],
        ];

        return new JsonResponse($data);
    }


    #[Route('/meteo-form/calculate', name: 'meteo_form_calculate')]
    public function calculate(Request $request): JsonResponse
    {

        $filePath = $this->getParameter('kernel.project_dir') . '/public/out/calc_form_meteo_output.txt';

        if (!file_exists($filePath)) {
            return new JsonResponse(['error' => 'Fichier non trouvé'], Response::HTTP_NOT_FOUND);
        }

        $lines = file($filePath, FILE_IGNORE_NEW_LINES | FILE_SKIP_EMPTY_LINES);
        dump($lines);
        if (count($lines) < 32) {
            return new JsonResponse(['error' => 'Fichier incomplet'], Response::HTTP_BAD_REQUEST);
        }

        $data = [
            'fileYears' => $lines[0],
            'sodiumChlorideConcentration' => $lines[1],
            'waterFilmThickness' => $lines[2],
            'humidityThreshold' => $lines[3],
            'mechanicalAnnualSodium' => $lines[4],
            'mechanicalMeanSodium' => $lines[5],
            'mechanicalInterventions' => $lines[6],
            'mechanicalInterval' => $lines[7],
            'mechanicalSodiumWater' => $lines[8],
            'mechanicalThresholdTemperature' => $lines[9],
            'automaticAnnualSodium' => $lines[10],
            'automaticMeanSodium' => $lines[11],
            'automaticSprays' => $lines[12],
            'automaticSprayInterval' => $lines[13],
            'automaticSodiumWater' => $lines[14],
            'automaticThresholdTemperature' => $lines[15],
            'extTemperaturePosition' => $lines[16],
            'extTemperaturePosition2' => $lines[17],
            'extTemperatureAttenuation' => $lines[18],
            'extTemperatureAttenuation2' => $lines[19],
            'extTemperatureDifference' => $lines[20],
            'extHumidityPosition' => $lines[21],
            'extHumidityPosition2' => $lines[22],
            'extHumidityAttenuation' => $lines[23],
            'extHumidityAttenuation2' => $lines[24],
            'extHumidityDifference' => $lines[25],
            'intTemperaturePosition' => $lines[26],
            'intTemperaturePosition2' => $lines[27],
            'intTemperatureAttenuation' => $lines[28],
            'intTemperatureAttenuation2' => $lines[29],
            'intTemperatureDifference' => $lines[30],
            'intHumidityPosition' => $lines[31],
            'intHumidityPosition2' => $lines[32],
            'intHumidityAttenuation' => $lines[33],
            'intHumidityAttenuation2' => $lines[34],
            'intHumidityDifference' => $lines[35],
        ];

        return new JsonResponse($data);
    }


    public function sendFile(String $fileName): Response
    {

        $filePath = $this->getParameter('kernel.project_dir') . '/public/meteoFiles/' . $fileName;

        $file = fopen($filePath, 'r');

        $client = HttpClient::create();
        $response = $client->request('POST', 'http://localhost:5000/precalcul', [
            'headers' => [
                'Content-Type' => 'multipart/form-data'
            ],
            'body' => [
                'file' => $file
            ]
        ]);
        return new Response($response->getContent(), $response->getStatusCode());
    }
}
