<?php

namespace App\Service;

use Symfony\Component\HttpClient\HttpClient;
use Symfony\Component\HttpFoundation\File\UploadedFile;
use Symfony\Component\HttpFoundation\JsonResponse;
use Symfony\Component\HttpFoundation\Response;
use Symfony\Contracts\HttpClient\HttpClientInterface;

class ApiService
{
    private $httpClient;
    private $apiUrl;
    private $meteoFilesDirectory;
    private $outputDirectory;
//    private $apiKey; //Inutile pour l'instant mais sera ajoutée plus tard pour plus de sécurité (si besoin)

    public function __construct(HttpClientInterface $httpClient, $apiUrl, string $uploadDir){
        $this->httpClient = $httpClient;
        $this->apiUrl = $apiUrl;
        $this->meteoFilesDirectory = $uploadDir . '/Ressources/MeteoFiles';
        $this->outputDirectory = $uploadDir . '/Ressources/out';
    }

    public function sendFile(string $filePath, string $route): Response
    {
        $file = fopen($filePath, 'r');

        $response = $this->httpClient->request('POST', $this->apiUrl . $route, [
            'body' => ['file' => $file]
        ]);

        return new Response($response->getContent(), $response->getStatusCode());
    }

    public function runTroubleshoots(string $filePath): array
    {
        return [
            'troubleshoot1' => $this->sendFile($filePath, 'troubleshoot1'),
            'troubleshoot2' => $this->sendFile($filePath, 'troubleshoot2'),
        ];
    }

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

    public function uploadAndPrecalculate(UploadedFile $file, string $newFileName): JsonResponse
    {
        try {
            $filePath = $this->meteoFilesDirectory . '/' . $newFileName;
            $file->move($this->meteoFilesDirectory, $newFileName);

            $this->runTroubleshoots($filePath);

            $response = $this->sendFile($filePath, 'precalcul');

            if ($response->getStatusCode() !== 200) {
                return new JsonResponse(['error' => 'Erreur API: ' . $response->getContent()], 500);
            }

            $outputPath = $this->outputDir . '/form_meteo_output_' . $newFileName;
            file_put_contents($outputPath, $response->getContent());

            $data = $this->extractPrecalcValues($outputPath);

            unlink($outputPath); // Nettoyage

            return new JsonResponse($data);
        } catch (\Exception $e) {
            return new JsonResponse(['error' => 'Erreur lors de l\'upload: ' . $e->getMessage()], 500);
        }
    }

}