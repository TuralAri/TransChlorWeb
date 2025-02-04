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


/*
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




    }
*/


}
