<?php

namespace App\Controller;

use Symfony\Bundle\FrameworkBundle\Controller\AbstractController;
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
    public function add() : Response
    {
        return $this->render('inputs/add.html.twig', [

        ]);
    }

}