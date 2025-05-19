<?php

namespace App\Controller;

use App\Entity\AggregateType;
use Symfony\Bundle\FrameworkBundle\Controller\AbstractController;
use Symfony\Component\HttpFoundation\JsonResponse;
use Symfony\Component\Routing\Annotation\Route;

class AggregateTypeController extends AbstractController
{
    #[Route('/aggregate-type/{id}', name: 'get_aggregate_type', methods: ['GET'])]
    public function getAggregateType(AggregateType $aggregateType): JsonResponse
    {
        return new JsonResponse([
            'heatCapacity' => $aggregateType->getHeatCapacity(),
            'aggregateDensity' => $aggregateType->getAggregateDensity(),
        ]);
    }
}