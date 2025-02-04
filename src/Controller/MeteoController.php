<?php

namespace App\Controller;

use App\Entity\Meteo;
use App\Form\MeteoType;
use Symfony\Bundle\FrameworkBundle\Controller\AbstractController;
use Symfony\Component\HttpFoundation\Request;
use Symfony\Component\HttpFoundation\Response;
use Symfony\Component\HttpFoundation\JsonResponse;
use Symfony\Component\Routing\Annotation\Route;
use Exception;

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

    #[Route('/upload-meteo', name: 'upload_meteo')]
    public function uploadMeteo(Request $request): JsonResponse
    {
        $file = $request->files->get('file');
        
        if ($file) {
            $fileName = $file->getClientOriginalName();
            
            try {
                $file->move(
                    $this->getParameter('kernel.project_dir') . '/public/meteoFiles',
                    $fileName
                );
                
                return new JsonResponse([
                    'success' => true,
                    'fileName' => $fileName
                ]);
                
            } catch (Exception $e) {
                return new JsonResponse([
                    'error' => 'Erreur lors de l\'upload: ' . $e->getMessage()
                ], 500);
            }
        }
        
        return new JsonResponse(['error' => 'Aucun fichier reçu'], 400);
    }
}
