<?php

namespace App\Service;

use Symfony\Component\HttpClient\HttpClient;
use Symfony\Component\HttpFoundation\File\UploadedFile;
use Symfony\Component\HttpFoundation\JsonResponse;
use Symfony\Component\HttpFoundation\Response;
use Symfony\Contracts\HttpClient\ResponseInterface;

/**
 *
 */
class ApiService
{
    private $httpClient;
    private $apiUrl;
    private $meteoFilesDirectory;
    private $outputDirectory;
//    private $apiKey; //Inutile pour l'instant mais sera ajoutée plus tard pour plus de sécurité (si besoin)

    public function __construct($apiUrl, string $uploadDir){
        $this->httpClient = HttpClient::create();
        $this->apiUrl = $apiUrl;
        $this->meteoFilesDirectory = $uploadDir . '/Ressources/MeteoFiles';
        $this->outputDirectory = $uploadDir . '/Ressources/out';
    }

    /**
     * Using a filePath, will send a file toward a specific API endpoint
     * @param string $filePath
     * @param string $route
     * @return Response
     * @throws \Symfony\Contracts\HttpClient\Exception\ClientExceptionInterface
     * @throws \Symfony\Contracts\HttpClient\Exception\RedirectionExceptionInterface
     * @throws \Symfony\Contracts\HttpClient\Exception\ServerExceptionInterface
     * @throws \Symfony\Contracts\HttpClient\Exception\TransportExceptionInterface
     */
    public function sendFile(string $filePath, string $route): Response
    {
        error_log("file sended with var :  " . $route . '  ds  '.$filePath);
        error_log($this->apiUrl . '/' .$route);
        $file = fopen($filePath, 'r');
        $response = $this->httpClient->request('POST', 'http://localhost:5000/' . $route , [
            'headers' => [
                'Content-Type' => 'multipart/form-data'
            ],
            'body' => [
                'file' => $file
            ]
        ]);
        return new Response($response->getContent(), $response->getStatusCode());
    }


    // Dans ApiService

    /**
     *   Executes a series of troubleshooting checks on a weather file
     *   by sending it to specific API endpoints.
     * @param string $filePath
     * @return array
     * @throws \Symfony\Contracts\HttpClient\Exception\ClientExceptionInterface
     * @throws \Symfony\Contracts\HttpClient\Exception\RedirectionExceptionInterface
     * @throws \Symfony\Contracts\HttpClient\Exception\ServerExceptionInterface
     * @throws \Symfony\Contracts\HttpClient\Exception\TransportExceptionInterface
     */
    public function runTroubleshoots(string $filePath): array
    {
        $results = [];
        $responses = [
            'troubleshoot1' => $this->sendFile($filePath, 'api/troubleshoot/troubleshoot1'),
            'troubleshoot2' => $this->sendFile($filePath, 'api/troubleshoot/troubleshoot2'),
        ];

        foreach ($responses as $key => $response) {
            if ($response->getStatusCode() === 200) {
                $results[] = [
                    'type' => 'success',
                    'message' => "$key: " . $response->getContent()
                ];
            } else {
                $results[] = [
                    'type' => 'error',
                    'message' => "$key échoué: " . $response->getContent()
                ];
            }
        }

        return $results;
    }

    /**
     * Extracts specific precalculated values from a weather data output file.
     *  Extracted values:
     *  - Line 0: Total number of years represented in the data.
     *  - Line 4: Annual mechanical sodium value.
     *  - Line 8: Annual automatic sodium value.
     * @param string $filePath
     * @return array
     * @throws \Exception
     */
    public function extractPrecalcValues(string $filePath): array
    {
        $lines = file($filePath, FILE_IGNORE_NEW_LINES | FILE_SKIP_EMPTY_LINES);
        if (count($lines) < 32) {
            throw new \Exception('Fichier précalcul incomplet.');
        }

        return [
            'fileYears' => floatval(str_replace(',', '.', $lines[0])),
            'mechanicalAnnualSodium' => floatval(str_replace(',', '.', $lines[4])),
            'automaticAnnualSodium' => floatval(str_replace(',', '.', $lines[8])),
        ];
    }

    /**
     * Uploads a weather file to the server's meteo files directory.
     * @param UploadedFile $file
     * @param string $newFileName
     * @return Response
     */
    public function upload(UploadedFile $file, string $newFileName) :Response{
        try {
            $filePath = $this->meteoFilesDirectory . '/' . $newFileName;
            $file->move($this->meteoFilesDirectory, $newFileName);
        }catch (\Exception $exception){
            return new Response("Uploaded file couldn't be uploaded");
        }
        return new Response($filePath, 200);
    }

    /**
     * Sends a weather file to the precalculation endpoint on API and extracts key values from the result.
     * @param String $filePath
     * @param String $newFileName
     * @return Response
     * @throws \Symfony\Contracts\HttpClient\Exception\ClientExceptionInterface
     * @throws \Symfony\Contracts\HttpClient\Exception\RedirectionExceptionInterface
     * @throws \Symfony\Contracts\HttpClient\Exception\ServerExceptionInterface
     * @throws \Symfony\Contracts\HttpClient\Exception\TransportExceptionInterface
     */
    public function precalculate(String $filePath, String $newFileName): JsonResponse
    {
        try {
            $response = $this->sendFile($filePath, 'api/data/precalcul');

            if ($response->getStatusCode() !== 200) {
                return new JsonResponse(['error' => 'Erreur API: ' . $response->getContent()], 500);
            }

            $this->generateOutFolderIfNotExists();

            $outputPath = $this->outputDirectory . '/form_meteo_output_' . $newFileName;
            file_put_contents($outputPath, $response->getContent());

            $data = $this->extractPrecalcValues($outputPath);

            unlink($outputPath); // Nettoyage

            return new JsonResponse($data);
        } catch (\Exception $e) {
            return new JsonResponse('Erreur lors de l\'upload: ' . $e->getMessage(), 500);
        }
    }

    //FROM HERE FUNCTIONS USED IN EXPOSURE SERIES CONTROLLER

    public function sendFilesForCalculation(string $meteoFilePath, string $dataFilePath, string $route): Response
    {
        $tempMeteoFileName = uniqid('temp_') . '_' . basename($meteoFilePath);
        $tempMeteoFilePath = $this->meteoFilesDirectory . '/' . $tempMeteoFileName;

        //COPIE DU FICHIER METEO POUR L ENVOYER SOUS UN NOM UNIQUE (ENTRE UTILISATEURS)

        copy($meteoFilePath, $tempMeteoFilePath);

        $file1 = fopen($tempMeteoFilePath, 'r');
        $file2 = fopen($dataFilePath, 'r');

        $response = $this->httpClient->request('POST', $this->apiUrl . '/' . $route, [
            'headers' => ['Content-Type' => 'multipart/form-data'],
            'body' => [
                'file1' => $file1,
                'file2' => $file2
            ]
        ]);

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

    public function initAfterCalc(string $filePath): JsonResponse
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

    public function calculate(array $data, string $meteoFileName): JsonResponse
    {
        $this->generateOutFolderIfNotExists();

        $uniqueId = uniqid();
        $inputFile = $this->outputDirectory . '/form_meteo_input_' . $uniqueId . '.txt';
        $outputFile = $this->outputDirectory . '/calc_form_meteo_output_' . $uniqueId . '.txt';
        $meteoFilePath = $this->meteoFilesDirectory . '/' . $meteoFileName;

        $dataString = implode("\n", $data);
        file_put_contents($inputFile, $dataString);

        $response = $this->sendFilesForCalculation($meteoFilePath, $inputFile, 'api/data/calcul');

        if ($response->getStatusCode() !== 200) {
            return new JsonResponse(['error' => 'Erreur API: ' . $response->getContent()], 500);
        }

        file_put_contents($outputFile, $response->getContent());

        $jsonResponse = $this->initAfterCalc($outputFile);

        unlink($inputFile);
        unlink($outputFile);

        return $jsonResponse;
    }

    public function generateExpositions(array $data, string $meteoFileName): Response
    {
        $this->generateOutFolderIfNotExists();

        $uniqueId = uniqid();
        $inputFile = $this->outputDirectory . '/form_meteo_input_' . $uniqueId . '.txt';
        $meteoFilePath = $this->meteoFilesDirectory . '/' . $meteoFileName;

        $data = array_map(function ($value) {
            return is_float($value) || is_numeric($value) ? str_replace('.', ',', (string)$value) : $value;
        }, $data);

        file_put_contents($inputFile, implode("\n", $data));

        $response = $this->sendFilesForCalculation($meteoFilePath, $inputFile, 'api/exposure/export');
        unlink($inputFile);

        return $response;
    }

    public function startRandomComputing(string $computationId) :void
    {
        $this->httpClient->request('GET', $this->apiUrl . '/api/computing/run', [
            'query' => [
                'mode' => 'random',
                'computationId' => $computationId
            ]
        ]);
    }

    public function stopComputing($computationId) : ResponseInterface
    {
        $response = $this->httpClient->request('POST', $this->apiUrl . '/api/computing/cancel', [
            'query' => ['computationId' => $computationId]
        ]);

        return $response;
    }

    /**
     * Checks if the out folder in var/uploads/Ressources/out exists or not
     * if not, creates the folder
     * @return void
     */
    public function generateOutFolderIfNotExists() : void
    {
        if(!file_exists($this->outputDirectory)) {
            mkdir($this->outputDirectory, 0777, true);
        }
    }

}