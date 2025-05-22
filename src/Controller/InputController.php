<?php

namespace App\Controller;

use App\Entity\Input;
use App\Form\InputFormType;
use Symfony\Bundle\FrameworkBundle\Controller\AbstractController;
use Symfony\Component\HttpFoundation\Request;
use Symfony\Component\HttpFoundation\Response;
use Symfony\Component\Routing\Attribute\Route;

class InputController extends AbstractController
{
    #[Route('/inputs', name: 'inputs')]
    public function index() : Response
    {
        return $this->render('inputs/index.html.twig', [

        ]);
    }

    #[Route('/inputs/add', name: 'add_input')]
    public function add(Request $request) : Response
    {
        $input = new Input();
        $form = $this->createForm(InputFormType::class, $input);
        $form->handleRequest($request);
        if ($form->isSubmitted() && $form->isValid()) {
        }

        return $this->render('inputs/add.html.twig', [
            'form' => $form->createView(),
        ]);
    }

}