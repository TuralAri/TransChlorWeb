<?php

namespace App\Controller;

use App\Entity\AggregateType;
use App\Entity\Material;
use App\Entity\Permeability;
use App\Form\MaterialFormType;
use App\Form\WeatherStationFormType;
use Symfony\Component\HttpFoundation\JsonResponse;
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
            $material = $form->getData();
            dd($material);

        }

        return $this->render('materials/add.html.twig',[
            'form' => $form->createView(),
        ]);
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