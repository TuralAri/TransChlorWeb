<?php

namespace App\Controller;

use App\Entity\Input;
use App\Form\InputFormType;
use Doctrine\ORM\EntityManagerInterface;
use Knp\Component\Pager\PaginatorInterface;
use Symfony\Bundle\FrameworkBundle\Controller\AbstractController;
use Symfony\Component\HttpFoundation\Request;
use Symfony\Component\HttpFoundation\Response;
use Symfony\Component\Routing\Attribute\Route;

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

}