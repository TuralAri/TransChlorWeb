<?php

namespace App\Controller;

use App\Entity\Material;
use App\Form\MaterialFormType;
use App\Form\WeatherStationFormType;
use Symfony\Component\HttpFoundation\Request;
use Symfony\Bundle\FrameworkBundle\Controller\AbstractController;
use Symfony\Component\Routing\Attribute\Route;

class MaterialController extends AbstractController
{
    #[Route('/materials/add', name: 'add_material')]
    public function add(Request $request){
        $material = new Material();

        $form = $this->createForm(MaterialFormType::class, $material);
        $form->handleRequest($request);
        if($form->isSubmitted() && $form->isValid()){

        }

        return $this->render('materials/add.html.twig',[
            'form' => $form->createView(),
        ]);
    }
}