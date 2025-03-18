<?php

namespace App\Controller;

use App\Entity\ExportFile;
use App\Entity\ImportFile;
use App\Entity\Meteo;
use App\Form\ExportFileType;
use App\Form\ImportFileType;
use App\Form\MeteoFormType;
use App\Form\SaveFormType;
use App\Repository\MeteoRepository;
use Doctrine\Persistence\ManagerRegistry;
use PhpParser\Node\Expr\Array_;
use Symfony\Bundle\FrameworkBundle\Controller\AbstractController;
use Symfony\Component\HttpClient\HttpClient;
use Symfony\Component\HttpFoundation\JsonResponse;
use Symfony\Component\HttpFoundation\Request;
use Symfony\Component\HttpFoundation\Response;
use Symfony\Component\Routing\Annotation\Route;
use Exception;

class MeteoController extends AbstractController
{
    
    #[Route('/meteo', name: 'meteo_form')]
    public function index(Request $request,ManagerRegistry $doctrine, MeteoRepository $userRepository): Response
    {


        if (!$this->getUser()) {
            return $this->redirectToRoute('index');
        }


        $importFile = new ImportFile();
        $importForm = $this->createForm(ImportFileType::class, $importFile);
        $importForm->handleRequest($request);

        $meteo = new Meteo();
        $form = $this->createForm(MeteoFormType::class, $meteo);
        $form->handleRequest($request);

        $exportFile = new ExportFile();
        $exportForm = $this->createForm(ExportFileType::class, $exportFile);
        $exportForm->handleRequest($request);

        $saveForm = $this->createForm(SaveFormType::class);
        $saveForm->handleRequest($request);

        if($importForm->isSubmitted() && $importForm->isValid()) {
            $request->getSession()->set('importedFileName',$importForm->get('importFile')->getData()->getClientOriginalName());
            $response = $this->uploadMeteo($request);
            if($response->getStatusCode() === 200){
                $responseContent = json_decode(trim($response->getContent()), true);
                if (isset($responseContent[0]) && is_string($responseContent[0])) {
                    $responseContent = json_decode($responseContent[0], true);
                }
                $expectedKeys = $this->getExpectedKeys(32);

                foreach ($expectedKeys as $key) {
                    $form->get($key)->setData(floatval(str_replace(',', '.', $responseContent[$key])));
                }

                $this->addFlash('success', 'Fichier importé avec succès');
            }else {
                $this->addFlash('error', 'Erreur lors de l\'importation du fichier');
            }
        }



        if($form->isSubmitted() && $form->isValid()){
            $formData = $form->getData();
            $formDataArray = $this->getFormData($formData);


            $response = $this->calculate($formDataArray,$request->getSession()->get('importedFileName'));

            if($response->getStatusCode() == 200){
                $responseContent = json_decode(trim($response->getContent()), true);
                if (isset($responseContent[0]) && is_string($responseContent[0])) {
                    $responseContent = json_decode($responseContent[0], true);
                }

                $meteo = new Meteo();
                $form = $this->createForm(MeteoFormType::class, $meteo);


                $expectedKeys = $this->getExpectedKeys(36);


                foreach ($expectedKeys as $key) {
                    $form->get($key)->setData(floatval(str_replace(',', '.', $responseContent[$key])));
                }





                $this->addFlash('success', 'Calcul effectué avec succès');
            }





        }

        if($saveForm->isSubmitted() && $saveForm->isValid()) {

            $meteo = new Meteo();

            $formData = $form->getData();
            dd($formData);
            $formDataArray = $this->getFormData($formData);
            dump($formDataArray);
            $expectedKeys = $this->getExpectedKeys(36);
            dd($expectedKeys);
            foreach ($expectedKeys as $key) {
                $meteo->$key = $formDataArray[$key];
            }




            $entityManager = $doctrine->getManager();
            $entityManager->persist($meteo);
            $entityManager->flush();
            $this->addFlash('success', 'Données sauvegardées avec succès');
        }



        if($exportForm->isSubmitted() && $exportForm->isValid()) {
            $response = $this->export($request);
            if($response->getStatusCode() === 200){
                $this->addFlash('success', 'Fichier exporté avec succès ils sont dans le dossier /public/exports du projet');
            }else {
                $this->addFlash('error', 'Erreur lors de l\'exportation du fichier');
            }
        }




        return $this->render('meteo/index.html.twig', [
            'form' => $form->createView(),
            'importForm' => $importForm->createView(),
            'exportForm' => $exportForm->createView(),
            'saveForm' => $saveForm->createView(),
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
                $this->troobleshoot($fileName);
                $response = $this->sendFile($fileName, 'precalcul');
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
                $response = $this->init($outputFilePath);
                return New JsonResponse([
                        $response->getContent()
                    ], $response->getStatusCode());
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

        $data = $this->getData(32,$lines);
        return new JsonResponse($data);
    }


       public function sendFile(String $fileName,String $route): Response
    {
        $filePath = $this->getParameter('kernel.project_dir') . '/public/meteoFiles/' . $fileName;
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

    public function troobleshoot(String $fileName) : Response{

        $response = $this->sendFile($fileName,'troubleshoot1');

        if($response->getStatusCode() === 200) {
            $this->addFlash('success', $response->getContent());
        }

        $response = $this->sendFile($fileName,'troubleshoot2');
        if($response->getStatusCode() === 200) {
            $this->addFlash('success', $response->getContent());
        }

        return new Response('Hello');
    }


    public function calculate(array $data , String $meteoFileName ): JsonResponse
    {
        $outputFileName = 'form_meteo_output.txt';
        $outputFilePath = $this->getParameter('kernel.project_dir') . '/public/out/' . $outputFileName;



        $dataString = implode("\n", $data);
        file_put_contents($outputFilePath, $dataString);

        $response = $this->sendFileForCalc($meteoFileName,$outputFileName, 'calcul');
        if ($response->getStatusCode() === 200) {
            $responseContent = $response->getContent();
            $outputFileName = 'calc_form_meteo_output.txt';
            $outputFilePath = $this->getParameter('kernel.project_dir') . '/public/out/' . $outputFileName;
            file_put_contents($outputFilePath, $responseContent);

            $response = $this->initAfterCalc($outputFilePath);
            if($response->getStatusCode() === 200) {
                return new JsonResponse([
                    $response->getContent()
                ], $response->getStatusCode());
            }else {
                return new JsonResponse([
                    'error' => 'Erreur lors de l\'initialisation après calcul: ' . $response->getContent()
                ], 500);
            }


        }else {
            return new JsonResponse([
                'error' => 'Erreur lors de l\'envoi du fichier: ' . $response->getContent()
            ], 500);
        }
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

        $data = $this->getData(36,$lines);

        return new JsonResponse($data);

    }


    public function sendFileForCalc(String $meteoFileName ,String $fileName, String $route): Response
    {
        $filePath1 = $this->getParameter('kernel.project_dir') . '/public/meteoFiles/' . $meteoFileName;
        $file1 = fopen($filePath1, 'r');


        $filePath2 = $this->getParameter('kernel.project_dir') . '/public/out/' . $fileName;
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



    public function export(Request $request): Response
    {

        $response = $this->sendFileForCalc($request->getSession()->get('importedFileName'),'calc_form_meteo_output.txt','export');

        return new Response( $response->getStatusCode());

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

    public function getExpectedKeys(int $nb) : array
    {
        if ($nb == 36) {
            return [
                'fileYears', 'sodiumChlorideConcentration', 'waterFilmThickness',
                'humidityThreshold', 'mechanicalAnnualSodium', 'mechanicalMeanSodium',
                'mechanicalInterventions', 'mechanicalInterval', 'mechanicalSodiumWater',
                'mechanicalThresholdTemperature', 'automaticAnnualSodium', 'automaticMeanSodium',
                'automaticSprays', 'automaticSprayInterval', 'automaticSodiumWater',
                'automaticThresholdTemperature', 'extTemperaturePosition', 'extTemperaturePosition2',
                'extTemperatureAttenuation', 'extTemperatureAttenuation2', 'extTemperatureDifference',
                'extHumidityPosition', 'extHumidityPosition2', 'extHumidityAttenuation',
                'extHumidityAttenuation2', 'extHumidityDifference', 'intTemperaturePosition',
                'intTemperaturePosition2', 'intTemperatureAttenuation', 'intTemperatureAttenuation2',
                'intTemperatureDifference', 'intHumidityPosition', 'intHumidityPosition2',
                'intHumidityAttenuation', 'intHumidityAttenuation2', 'intHumidityDifference'
            ];
        } else if ($nb == 32) {
            return [
                'fileYears', 'sodiumChlorideConcentration', 'waterFilmThickness',
                'humidityThreshold', 'mechanicalAnnualSodium', 'mechanicalMeanSodium',
                'mechanicalInterval', 'mechanicalSodiumWater', 'automaticAnnualSodium',
                'automaticMeanSodium', 'automaticSprayInterval', 'automaticSodiumWater',
                'extTemperaturePosition', 'extTemperaturePosition2', 'extTemperatureAttenuation',
                'extTemperatureAttenuation2', 'extTemperatureDifference', 'extHumidityPosition',
                'extHumidityPosition2', 'extHumidityAttenuation', 'extHumidityAttenuation2',
                'extHumidityDifference', 'intTemperaturePosition', 'intTemperaturePosition2',
                'intTemperatureAttenuation', 'intTemperatureAttenuation2', 'intTemperatureDifference',
                'intHumidityPosition', 'intHumidityPosition2', 'intHumidityAttenuation',
                'intHumidityAttenuation2', 'intHumidityDifference'
            ];
        }
        return [];
    }

    public function getData(int $nb,$lines) : array {
        if($nb == 36){
            return [
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
        }else if($nb == 32){
            return [
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
        }
        return [];
    }


}
