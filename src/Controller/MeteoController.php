<?php

namespace App\Controller;

use App\Entity\Meteo;
use App\Form\MeteoType;
use Symfony\Bundle\FrameworkBundle\Controller\AbstractController;
use Symfony\Component\HttpClient\HttpClient;
use Symfony\Component\HttpFoundation\JsonResponse;
use Symfony\Component\HttpFoundation\Request;
use Symfony\Component\HttpFoundation\Response;
use Symfony\Component\Routing\Annotation\Route;
use Symfony\Contracts\HttpClient\HttpClientInterface;

class MeteoController extends AbstractController
{
    private HttpClientInterface $client;

    public function __construct(HttpClientInterface $client)
    {
        $this->client = $client;
    }
    #[Route('/meteo', name: 'meteo_form')]
    public function index(Request $request): Response
    {
        $meteo = new Meteo();
        $form = $this->createForm(MeteoType::class, $meteo);

        $form->handleRequest($request);

        if ($form->isSubmitted() && $form->isValid()) {
        }

        return $this->render('meteo/index.html.twig', [
            'form' => $form->createView(),
        ]);
    }


    #[Route('/upload-meteo', name: 'upload_meteo')]
    public function uploadMeteo(Request $request): JsonResponse
    {
        $file = $request->files->get('file');
        dump($file);
        if ($file) {
            $fileName = $file->getClientOriginalName();

            try {
                $file->move(
                    $this->getParameter('kernel.project_dir') . '/public/meteoFiles',
                    $fileName
                );
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

                return new JsonResponse([
                    'success' => true
                ]);

            } catch (\Exception $e) {
                return new JsonResponse([
                    'error' => 'Erreur lors de l\'upload: ' . $e->getMessage()
                ], 500);
            }
        }

        return new JsonResponse(['error' => 'Aucun fichier reçu'], 400);
    }

    #[Route('/meteo-form/init', name: 'meteo_form_init')]
    public function init(): JsonResponse
    {
        $filePath = $this->getParameter('kernel.project_dir') . '/public/out/form_meteo_output.txt';

        if (!file_exists($filePath)) {
            return new JsonResponse(['error' => 'Fichier non trouvé'], Response::HTTP_NOT_FOUND);
        }

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
        dump($fileName);
        $filePath = $this->getParameter('kernel.project_dir') . '/public/meteoFiles/' . $fileName;
        dump($filePath);
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
        dump("after request");
        return new Response($response->getContent(), $response->getStatusCode());
    }

    #[Route('/test', name: 'send_file')]
    public function test(): Response
    {
        $filePath1 = __DIR__ . '/../../public/meteoFiles/METEO_DAVOS.TXT'; 
        $file1 = fopen($filePath1, 'r');

        $response = $this->client->request('POST', 'http://localhost:5000/troubleshoot2', [
            'headers' => [
                'Content-Type' => 'multipart/form-data'
            ],
            'body' => [
                'file1' => $file1,
            ]
        ]);

        return new Response('Réponse du serveur VB: ' . $response->getContent());
    }

    #[Route('/test2', name: 'test2')]
    public function test2(): Response
    {
        $filePath1 = __DIR__ . '/../../public/meteoFiles/METEO_DAVOS.TXT'; 
        $filePath2 = __DIR__ . '/../../public/formFiles/TempSeuil.txt';
        $file1 = fopen($filePath1, 'r');
        $file2 = fopen($filePath2, 'r');

        $response = $this->client->request('POST', 'http://localhost:5000/export', [
            'headers' => [
                'Content-Type' => 'multipart/form-data'
            ],
            'body' => [
                'file1' => $file1,
                'file2' => $file2
            ]
        ]);

        return new Response('Réponse du serveur VB: ' . $response->getContent());
    }

}
