<?php

namespace App\Controller;

use App\Entity\Input;
use App\Form\InputFormType;
use App\Repository\InputRepository;
use Doctrine\ORM\EntityManagerInterface;
use http\Exception\RuntimeException;
use Knp\Component\Pager\PaginatorInterface;
use Symfony\Bundle\FrameworkBundle\Controller\AbstractController;
use Symfony\Component\HttpFoundation\BinaryFileResponse;
use Symfony\Component\HttpFoundation\Request;
use Symfony\Component\HttpFoundation\Response;
use Symfony\Component\HttpFoundation\ResponseHeaderBag;
use Symfony\Component\Routing\Attribute\Route;
use Symfony\Contracts\Translation\TranslatorInterface;

class InputController extends AbstractController
{
    #[Route('/inputs', name: 'inputs')]
    public function index(Request $request, PaginatorInterface $paginator) : Response
    {
        $user = $this->getUser();
        if(!$user){
            return $this->redirectToRoute('index');
        }

        $query = $user->getInputs();

        $inputs = $paginator->paginate(
            $query,
            $request->query->getInt('page', 1),
            10
        );

        return $this->render('inputs/index.html.twig', [
            'inputs' => $inputs,
        ]);
    }

    #[Route('/inputs/add', name: 'add_input')]
    public function add(Request $request, EntityManagerInterface $entityManager) : Response
    {
        $user = $this->getUser();
        if(!$user){
            return $this->redirectToRoute('index');
        }

        $input = new Input();
        $form = $this->createForm(InputFormType::class, $input);
        $form->handleRequest($request);
        if ($form->isSubmitted() && $form->isValid()) {
            $input = $form->getData();
            $input->setUser($user);
            $entityManager->persist($input);
            $entityManager->flush();
            return $this->redirectToRoute('inputs');
        }

        return $this->render('inputs/add.html.twig', [
            'form' => $form->createView(),
        ]);
    }

    #[Route('/inputs/{id}/edit', name: 'edit_input')]
    public function edit(Input $input, Request $request, EntityManagerInterface $entityManager) : Response
    {
        $user = $this->getUser();
        if(!$user || $input->getUser()->getId() != $user->getId()){
            return $this->redirectToRoute('index');
        }

        $form = $this->createForm(InputFormType::class, $input);
        $form->handleRequest($request);
        if ($form->isSubmitted() && $form->isValid()) {
            $input = $form->getData();
            $entityManager->persist($input);
            $entityManager->flush();
            return $this->redirectToRoute('inputs');
        }

        return $this->render('inputs/add.html.twig', [
            'form' => $form->createView(),
        ]);
    }

    #[Route('/inputs/{id}/delete', name: 'delete_input', methods: ['POST'])]
    public function delete(Request $request, Input $input, EntityManagerInterface $entityManager, TranslatorInterface $translator): Response
    {
        $user = $this->getUser();
        if(!$user){
            return $this->redirectToRoute('index');
        }

        if ($this->isCsrfTokenValid('delete' . $input->getId(), $request->request->get('_token')) && $input->getUser() === $user) {
            $entityManager->remove($input);
            $entityManager->flush();

            $this->addFlash('success', $translator->trans('input.deleteSuccess'));
        } else {
            $this->addFlash('error', $translator->trans('input.deleteError'));
        }

        return $this->redirectToRoute('inputs');
    }

    #[Route('/inputs/{id}/generate', name: 'generate_input')]
    public function generateInput(Input $input): Response
    {
        return $this->writeInputFile($input);
    }

    public function writeInputFile(Input $input) : Response
    {
        $materials = $input->getMaterial();
        $tempMat = $materials->get(0); //Variable pour les valeurs d'input tant qu'on a pas fixé le probleme des variables dans les mauvaises entitées.

        $uploadDirectory = $this->getParameter('upload_directory') . '/Ressources/Input/';

        if (!is_dir($uploadDirectory)) {
            mkdir($uploadDirectory, 0777, true);
        }

        $filename = 'input_' . $input->getId() . '.txt';
        $filePath = $uploadDirectory . $filename;

        $handle = fopen($filePath, "w");

        if($handle === false){
            throw new RuntimeException();
        }

        fwrite($handle, $input->getWallThickness() . "\n"); //Length
        fwrite($handle, $input->getElementsNumber() . "\n"); //Ne

        switch ($input->getMeshType()) {
            case 1:
                break; //Nothing
        }

        fwrite($handle, $input->getMaxComputingTime() . "\n"); //TimeMax
        fwrite($handle, $input->getComputingTimeStep() . "\n"); //DeltaT
        fwrite($handle, $input->getResultsDisplayTime() . "\n"); //taff
        fwrite($handle, $input->getSaveTimeRelativeHumidity() . "\n"); //Hsauv
        fwrite($handle, $input->getSaveTimeWaterContent() . "\n"); //WSauv
        fwrite($handle, $input->getSaveTimeTotalChlorures() . "\n"); //CTSauv
        fwrite($handle, $input->getSaveTimeFreeChlorures() . "\n"); //CLSauv
        fwrite($handle, $input->getSaveTimeTemperature() . "\n"); //TSauv
        fwrite($handle, $input->getSaveTimePh() . "\n"); //Carbsauv
        fwrite($handle, 0 . "\n"); //hMin
        fwrite($handle, 0 . "\n"); //hEcart
        fwrite($handle, 0 . "\n"); //wMin
        fwrite($handle, 0 . "\n"); //wEcart
        fwrite($handle, 0 . "\n"); //CTmin
        fwrite($handle, 0 . "\n"); //CTecart
        fwrite($handle, 0 . "\n"); //CLmin
        fwrite($handle, 0 . "\n"); //CLecart
        fwrite($handle, 0 . "\n"); //Tecart
        fwrite($handle, $tempMat->getAoDiffusion() . "\n"); //aa
        fwrite($handle, $tempMat->getHc() . "\n"); //hc
        fwrite($handle, $tempMat->getAoCapillarity() . "\n"); //ab
        fwrite($handle, $tempMat->getTc() . "\n"); //tc
        switch ($input->getCapillarityTreatment()) { //ImpHydr
            case 1:
                fwrite($handle, "true" . "\n");
                break;
            case 2:
                fwrite($handle, "false" . "\n");
                break;
        }
        fwrite($handle, $tempMat->getLimitWaterContent() . "\n"); //H_Snap
        fwrite($handle, $tempMat->getRetardationCoefficient() . "\n"); //Retard
        fwrite($handle, $tempMat->getAlphaOh() . "\n"); //aOH
        fwrite($handle, $tempMat->getEb() . "\n"); //EbG
        fwrite($handle, $tempMat->getToAdsorption() . "\n"); //toG
        fwrite($handle, $tempMat->getAdsorptionFa() . "\n"); //faG
        fwrite($handle, $tempMat->getHeatCapacity() . "\n"); //capCal
        fwrite($handle, $input->getLeftEdgeCO2() . "\n"); //GyCO2
        fwrite($handle, $input->getRightEdgeCO2() . "\n"); //DyCO2
        fwrite($handle, "2" . "\n"); //Number of expo files NEXPO

        for($i = 0; $i < 2; $i++) {
            fwrite($handle, $i . "Fichier expo virtuel" . "\n");
        }

        fwrite($handle, $materials->count() . "\n");//Var03 number of materials
        foreach($materials as $material){
            fwrite($handle, $material->getName() . "\n"); //Nom d'affichage durant le calcul
            fwrite($handle, $material->getName() . "\n"); //nom dans le fichier résultat
            fwrite($handle, $material->getD100Percent() . "\n");//coefficient de diffusion hydrique
            fwrite($handle, $material->getDclTo() . "\n"); //coefficient de diffusion des ions chlorures dans l'eau
            fwrite($handle, $material->getAggregateContent() . "\n"); //Teneur en granulat
            fwrite($handle, $material->getSurfaceTransferCoefficient() . "\n");//coefficient de transfert de surface pour l'eau
            fwrite($handle, $material->getSurfaceHeatTransfer() . "\n");//coefficient de transfert de surface pour la température
            fwrite($handle, $material->getSaturatedWaterContent() . "\n");//teneur en eau saturée
            fwrite($handle, $material->getCementContent() . "\n");//quantité de ciment
            fwrite($handle, $material->getEc() . "\n");//eau sur ciment
            fwrite($handle, $material->getConcreteAge() . "\n");//age du béton
            fwrite($handle, $material->getHydrationRate() . "\n");//taux d'hydratation
            //

            //
            fwrite($handle, $material->getEd() . "\n");//énergie d'activation pour la vapeur d'eau (température)
            fwrite($handle, $material->getToDiffusion() . "\n");//température de référence pour l'énergie d'activation précédente
            fwrite($handle, $material->getAlphaDiffusion() . "\n");//énergie d'activation pour l'entraînement des ions cl- par l'eau (température)
            fwrite($handle, $material->getToChlorideDiffusion() . "\n");//température de référence pour l'énergie d'activation précédente
            fwrite($handle, $material->getAggregateDensity() . "\n");//masse volumique des granulats
            fwrite($handle, $material->getCementDensity() . "\n");//masse volumique du ciment
            //


            //
            fwrite($handle, $material->getEc() . "\n");//eau sur ciment pour le calcul Dcap // CORRESPOND AU E/C VIRTUEL


        }

        fclose($handle);

        $response = new BinaryFileResponse($filePath);
        $response->setContentDisposition(
            ResponseHeaderBag::DISPOSITION_ATTACHMENT,
            $filename
        );

        return $response;
    }

}