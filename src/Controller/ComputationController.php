<?php

namespace App\Controller;

use App\Entity\Computation;
use App\Entity\ComputationResult;
use App\Repository\ComputationRepository;
use App\Repository\ComputationResultRepository;
use App\Service\ApiService;
use Doctrine\ORM\EntityManagerInterface;
use Symfony\Bundle\FrameworkBundle\Controller\AbstractController;
use Symfony\Component\HttpFoundation\JsonResponse;
use Symfony\Component\HttpFoundation\Request;
use Symfony\Component\Routing\Annotation\Route;

class ComputationController extends AbstractController
{

    private ApiService $apiService;
    public function __construct(ApiService $apiService)
    {
        $this->apiService = $apiService;
    }

    #[Route('/api/computations/random', name: 'start_random', methods: ['GET'])]
    public function startRandom(EntityManagerInterface $entityManager) : JsonResponse{
        $computation = new Computation();
        $computation->setStartDate(new \DateTime());
        $computation->setEndDate(new \DateTime());
        $computation->setStatus("progress"); //staus is whether in progess or ended
        $entityManager->persist($computation);
        $entityManager->flush();

        //call api to start computation
        $this->apiService->startRandomComputing($computation->getId());

        return $this->json([
            'computationId' => $computation->getId(),
            'status' => $computation->getStatus(),
        ],201);
    }

    #[Route('/api/computations-results', name: 'receive_results', methods: ['POST'])]
    public function receiveResult(Request $request, EntityManagerInterface $entityManager, ComputationRepository $computationRepository): JsonResponse
    {
        $data = json_decode($request->getContent(), true);

        $computation = $computationRepository->findOneBy(['id' => $data['computationId']]);
        if(!$computation){
            return $this->json(['error' => 'Computation not found'], 404);
        }

        $result = new ComputationResult();
        $result->setTime((int) $data['time'])
               ->setDepths($data['depths'])
               ->setComputedValues($data['values'])
               ->setType($data['type'])
               ->setComputation($computation);

        $entityManager->persist($result);

        //Condition to change computation state
        // if(completed)....
        //

        $entityManager->flush();

        return $this->json(['success' => true],201);
    }

    #[Route('computation/{id}/show', name: 'show_computation')]
    public function show(Computation $computation, EntityManagerInterface $entityManager, ComputationResultRepository $computationResultRepository)
    {
        $types = [
            "temperature_potential",
            "moisture_potential",
            "moisture_content",
            "total_chloride",
            "free_chloride",
            "ph"
        ];

        $graphData = [];

        foreach ($types as $type) {
            $result = $computationResultRepository->findOneBy(['computation' => $computation, 'type' => $type], ['id' => 'DESC']);
            if (!$result) continue;

            $graphData[] = [
                'type' => $type,
                'data' => array_map(fn($depth, $val) => ['x' => $depth, 'y' => $val], $result->getDepths(), $result->getComputedValues()),
                'borderColor' => '#0000ff',
                'fill' => false,
                'tension' => 0.3
            ];
        }

        return $this->render('computation/show.html.twig', [
            'computation' => $computation,
            'datasets' => $graphData,
        ]);
    }


}