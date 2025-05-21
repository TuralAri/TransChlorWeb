<?php

namespace App\Controller;

use App\Entity\AggregateType;
use App\Entity\Material;
use App\Entity\Permeability;
use App\Form\MaterialFormType;
use App\Form\WeatherStationFormType;
use Doctrine\ORM\EntityManagerInterface;
use Knp\Component\Pager\PaginatorInterface;
use Symfony\Component\HttpFoundation\JsonResponse;
use Symfony\Component\HttpFoundation\Request;
use Symfony\Bundle\FrameworkBundle\Controller\AbstractController;
use Symfony\Component\HttpFoundation\Response;
use Symfony\Component\Routing\Attribute\Route;
use Symfony\Contracts\Translation\TranslatorInterface;

class MaterialController extends AbstractController
{
    #[Route('/materials', name: 'materials')]
    public function index(Request $request, PaginatorInterface $paginator)
    {
        $user = $this->getUser();
        if(!$user){
            return $this->redirectToRoute('index');
        }

        $query = $user->getMaterials();

        $materials = $paginator->paginate(
            $query,
            $request->query->getInt('page', 1),
            10
        );

        return $this->render('materials/index.html.twig', [
            'materials' => $materials,
        ]);
    }

    #[Route('/materials/add', name: 'add_material')]
    public function add(Request $request, EntityManagerInterface $entityManager, TranslatorInterface $translator) : Response{
        $user = $this->getUser();
        if(!$user){
            return $this->redirectToRoute('index');
        }

        $material = new Material();

        $form = $this->createForm(MaterialFormType::class, $material);
        $form->handleRequest($request);
        if($form->isSubmitted() && $form->isValid()){
            if ($form->get('submit')->isClicked()) {
                $material = $form->getData();
//                dd($material);
                $material->setUser($this->getUser());
                $entityManager->persist($material);
                $entityManager->flush();
                $this->addFlash("success", $translator->trans("materials.addSuccess"));
                return $this->redirectToRoute("materials");
            }
        }

        if($form->isSubmitted() && !$form->isValid()){
            $this->addFlash("error", $translator->trans("materials.addError"));
            return $this->redirectToRoute("add_material");
        }

        return $this->render('materials/add.html.twig',[
            'form' => $form->createView(),
        ]);
    }

    #[Route('/materials/edit/{id}', name: 'edit_material')]
    public function edit(Material $material, Request $request, EntityManagerInterface $entityManager, TranslatorInterface $translator) : Response{
        $user = $this->getUser();
        if(!$user || $material->getUser() !== $user){
            return $this->redirectToRoute('materials');
        }

        $form = $this->createForm(MaterialFormType::class, $material);
        $form->handleRequest($request);
        if($form->isSubmitted() && $form->isValid()){
            $material = $form->getData();
            $entityManager->persist($material);
            $entityManager->flush();
            $this->addFlash("success", $translator->trans("materials.editSuccess"));
            return $this->redirectToRoute("materials");
        }
        if($form->isSubmitted() && !$form->isValid()){
            $this->addFlash("error", $translator->trans("materials.editError"));
            return $this->redirectToRoute("edit_material", ["id" => $material->getId()]);
        }

        return $this->render('materials/edit.html.twig',[
            'form' => $form->createView(),
        ]);
    }

    #[Route('/materials/{id}/delete', name: 'delete_material', methods: ['POST'])]
    public function delete(Request $request, Material $material, EntityManagerInterface $entityManager, TranslatorInterface $translator): Response
    {
        $user = $this->getUser();
        if(!$user){
            return $this->redirectToRoute('index');
        }

        if ($this->isCsrfTokenValid('delete' . $material->getId(), $request->request->get('_token')) && $material->getUser() === $user) {
            $entityManager->remove($material);
            $entityManager->flush();

            $this->addFlash('success', $translator->trans('materials.deleteSuccess'));
        } else {
            $this->addFlash('error', $translator->trans('materials.deleteError'));
        }

        return $this->redirectToRoute('materials');
    }


    //Route that permits asking for the heatCapacity and aggregateDensity from a an aggregateType
    #[Route('/aggregate-type/{id}', name: 'get_aggregate_type', methods: ['GET'])]
    public function getAggregateType(AggregateType $aggregateType): JsonResponse
    {
        return new JsonResponse([
            'heatCapacity' => $aggregateType->getHeatCapacity(),
            'aggregateDensity' => $aggregateType->getAggregateDensity(),
        ]);
    }

    //Route that permits asking for the data from
    #[Route('/permeability/{id}', name: 'get_permeability', methods: ['GET'])]
    public function getPermeability(Permeability $permeability): JsonResponse
    {
        return new JsonResponse([
            'name' => $permeability->getName(),
            'd100Percent' => $permeability->getD100Percent(),
            'dclTo' => $permeability->getDclTo(),
            'heatCapacity' => $permeability->getHeatCapacity(),
            'surfaceTransferCoefficient' => $permeability->getSurfaceTransferCoefficient(),
            'cementContent' => $permeability->getCementDensity(),
            'ec' => $permeability->getEc(),
            'freshConcreteDensity' => $permeability->getFreshConcreteDensity(),
            'hydrationRate' => $permeability->getHydrationRate(),
            'airContent' => $permeability->getAirContent(),
            'ed' => $permeability->getEd(),
            'toDiffusion' => $permeability->getToDiffusion(),
            'toChlorideDiffusion' => $permeability->getToChlorideDiffusion(),
        ]);

    }
}