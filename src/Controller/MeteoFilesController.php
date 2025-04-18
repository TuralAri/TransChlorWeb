<?php

namespace App\Controller;

use App\Entity\MeteoFile;
use App\Form\MeteoFileFormType;
use App\Repository\MeteoFileRepository;
use Doctrine\ORM\EntityManager;
use Doctrine\ORM\EntityManagerInterface;
use Symfony\Bundle\FrameworkBundle\Controller\AbstractController;
use Symfony\Component\HttpFoundation\File\Exception\FileException;
use Symfony\Component\HttpFoundation\Request;
use Symfony\Component\Routing\Annotation\Route;

class MeteoFilesController extends AbstractController
{
    #[Route('/meteofiles', name: 'meteo_files')]
    public function index()
    {
        if(!$this->getUser()){
            return $this->redirectToRoute('index');
        }

        return $this->render('meteofiles/index.html.twig', []);
    }

    #[Route('/meteofiles/add', name: 'meteo_files_add')]
    public function add(EntityManagerInterface $em, Request $request, MeteoFileRepository $repository)
    {
        if(!$this->getUser()){
            return $this->redirectToRoute('index');
        }
        $meteoFile = new MeteoFile();
        $form = $this->createForm(MeteoFileFormType::class, $meteoFile);
        $form->handleRequest($request);

        if($form->isSubmitted() && $form->isValid() && $request->getMethod() == 'POST'){
            $uploadMeteoFile = $form->get('filename')->getData();

            $meteoFile = $form->getData();
            $meteoFile->setUploadedBy($this->getUser());
            $meteoFile->setUploadedAt(new \DateTimeImmutable('now'));

            $newFileName = time() . '.' . $uploadMeteoFile->guessExtension();

            $uploadDirectory = $this->getParameter('upload_directory') . '/Ressources/MeteoFiles';

            try{
                $uploadMeteoFile->move($uploadDirectory, $newFileName);
            }catch (FileException $e){
                //On verra ça plus tard
            }

            $meteoFile->setFilename($newFileName);

            $em->persist($meteoFile);
            $em->flush();
            return $this->redirectToRoute('meteo_files');
        }

        return $this->render('meteofiles/add.html.twig', [
            'form' => $form->createView(),
        ]);
    }

}