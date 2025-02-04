<?php

namespace App\Controller;

use App\Entity\Meteo;
use App\Form\MeteoType;
use Symfony\Bundle\FrameworkBundle\Controller\AbstractController;
use Symfony\Component\HttpFoundation\Request;
use Symfony\Component\HttpFoundation\Response;
use Symfony\Component\Routing\Annotation\Route;

class MeteoController extends AbstractController
{
    
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



    #Route('/meteo-form/init', name: 'meteo_form_init')
    public function init(): void
    {
        $response = $this->client->request('GET', 'http://localhost:5000/', [
            'headers' => [
                'Content-Type' => 'multipart/form-data',
            ],
            'body' => []
        ]);

        $content = $response->getContent();
        $filePath = '../../public/meteo_form.txt';

        file_put_contents($filePath, $content);

        $lines = file($filePath, FILE_IGNORE_NEW_LINES | FILE_SKIP_EMPTY_LINES);

        $meteo = new Meteo();

        $meteo->setFileYears($lines[0]);
        $meteo->setSodiumChlorideConcentration($lines[1]);
        $meteo->setWaterFilmThickness($lines[2]);
        $meteo->setHumidityThreshold($lines[3]);

        $meteo->setMechanicalAnnualSodium($lines[4]);
        $meteo->setMechanicalMeanSodium($lines[5]);
        $meteo->setMechanicalInterval($lines[6]);
        $meteo->setMechanicalSodiumWater($lines[7]);

        $meteo->setAutomaticAnnualSodium($lines[8]);
        $meteo->setAutomaticMeanSodium($lines[9]);
        $meteo->setAutomaticSprayInterval($lines[10]);
        $meteo->setAutomaticSodiumWater($lines[11]);

        $meteo->setExtTemperaturePosition($lines[12]);
        $meteo->setExtTemperaturePosition2($lines[13]);
        $meteo->setExtTemperatureAttenuation($lines[14]);
        $meteo->setExtTemperatureAttenuation2($lines[15]);
        $meteo->setExtTemperatureDifference($lines[16]);
        $meteo->setExtHumidityPosition($lines[17]);
        $meteo->setExtHumidityPosition2($lines[18]);
        $meteo->setExtHumidityAttenuation($lines[19]);
        $meteo->setExtHumidityAttenuation2($lines[20]);
        $meteo->setExtHumidityDifference($lines[21]);

        $meteo->setIntTemperaturePosition($lines[22]);
        $meteo->setIntTemperaturePosition2($lines[23]);
        $meteo->setIntTemperatureAttenuation($lines[24]);
        $meteo->setIntTemperatureAttenuation2($lines[25]);
        $meteo->setIntTemperatureDifference($lines[26]);
        $meteo->setIntHumidityPosition($lines[27]);
        $meteo->setIntHumidityPosition2($lines[28]);
        $meteo->setIntHumidityAttenuation($lines[29]);
        $meteo->setIntHumidityAttenuation2($lines[30]);
        $meteo->setIntHumidityDifference($lines[31]);

    }



}
