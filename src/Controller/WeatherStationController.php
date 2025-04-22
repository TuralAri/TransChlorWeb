<?php

namespace App\Controller;

use App\Entity\WeatherStation;
use App\Form\WeatherStationFormType;
use App\Repository\WeatherStationRepository;
use Doctrine\ORM\EntityManager;
use Doctrine\ORM\EntityManagerInterface;
use Symfony\Bundle\FrameworkBundle\Controller\AbstractController;
use Symfony\Component\HttpFoundation\File\Exception\FileException;
use Symfony\Component\HttpFoundation\Request;
use Symfony\Component\Routing\Annotation\Route;

class WeatherStationController extends AbstractController
{
    #[Route('/weatherstations', name: 'weather_stations')]
    public function index(WeatherStationRepository $repository)
    {
        if(!$this->getUser()){
            return $this->redirectToRoute('index');
        }

        $meteoFiles = $this->getUser()->getMeteoFiles();

        return $this->render('weatherstations/index.html.twig', [
            'meteoFiles' => $meteoFiles,
        ]);
    }

    #[Route('/weatherstations/add', name: 'weather_station_add')]
    public function add(EntityManagerInterface $em, Request $request, WeatherStationRepository $repository)
    {
        if(!$this->getUser()){
            return $this->redirectToRoute('index');
        }
        $weatherStation = new WeatherStation();
        $form = $this->createForm(WeatherStationFormType::class, $weatherStation);
        $form->handleRequest($request);

        if($form->isSubmitted() && $form->isValid() && $request->getMethod() == 'POST'){
            $uploadMeteoFile = $form->get('filename')->getData();

            $weatherStation = $form->getData();
            $weatherStation->setLocalFileName($uploadMeteoFile->getClientOriginalName());
            $weatherStation->setUploadedBy($this->getUser());
            $weatherStation->setUploadedAt(new \DateTimeImmutable('now'));

            $newFileName = time() . '.' . $uploadMeteoFile->guessExtension();

            $uploadDirectory = $this->getParameter('upload_directory') . '/Ressources/MeteoFiles';

            try{
                $uploadMeteoFile->move($uploadDirectory, $newFileName);
            }catch (FileException $e){
                //On verra ça plus tard
            }

            $weatherStation->setFilename($newFileName);

            $em->persist($weatherStation);
            $em->flush();
            return $this->redirectToRoute('weather_stations');
        }

        return $this->render('weatherstations/add.html.twig', [
            'form' => $form->createView(),
        ]);
    }

}