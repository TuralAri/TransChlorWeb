<?php

namespace App\Controller;

use App\Entity\ExposureSeries;
use App\Entity\WeatherStation;
use App\Form\ExposureSeriesFormType;
use App\Repository\ExposureSeriesRepository;
use Symfony\Bundle\FrameworkBundle\Controller\AbstractController;
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
    public function generate(WeatherStation $weatherStation, ExposureSeriesRepository $exposureSeriesRepository) : Response
    {
        $exposureSeries = new ExposureSeries();
        $exposureSeries->setWeatherStation($weatherStation);
        $exposureSeries->setFileYears($weatherStation->getFileYears());
        $exposureSeries->setMechanicalAnnualSodium($weatherStation->getMechanicalAnnualSodium());
        $exposureSeries->setAutomaticAnnualSodium($weatherStation->getAutomaticAnnualSodium());

        $exposureSeriesForm = $this->createForm(ExposureSeriesFormType::class, $exposureSeries);
        $exposureSeriesForm->handleRequest($request);

        if ($exposureSeriesForm->isSubmitted() && $exposureSeriesForm->isValid()) {

        }

        return $this->render('exposure_series/generate.html.twig', [
            'weatherStation' => $weatherStation,
            'exposureSeriesForm' => $exposureSeriesForm->createView(),
        ]);
    }

}