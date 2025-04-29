<?php

namespace App\Service;

use Symfony\Component\HttpClient\HttpClient;
use Symfony\Component\HttpFoundation\File\UploadedFile;
use Symfony\Component\HttpFoundation\JsonResponse;
use Symfony\Component\HttpFoundation\Response;

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

            $outputPath = $this->outputDirectory . '/form_meteo_output_' . $newFileName;
            file_put_contents($outputPath, $response->getContent());

            $data = $this->extractPrecalcValues($outputPath);

            unlink($outputPath); // Nettoyage

            return new JsonResponse($data);
        } catch (\Exception $e) {
            return new JsonResponse('Erreur lors de l\'upload: ' . $e->getMessage(), 500);
        }
    }

}