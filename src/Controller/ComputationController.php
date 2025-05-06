<?php

namespace App\Controller;

use App\Entity\Computation;
use App\Entity\ComputationActualResult;
use App\Entity\ComputationResult;
use App\Repository\ComputationActualResultRepository;
use App\Repository\ComputationRepository;
use App\Repository\ComputationResultRepository;
use App\Service\ApiService;
use Doctrine\ORM\EntityManagerInterface;
use Knp\Component\Pager\PaginatorInterface;
use Symfony\Bundle\FrameworkBundle\Controller\AbstractController;
use Symfony\Component\HttpFoundation\JsonResponse;
use Symfony\Component\HttpFoundation\Request;
use Symfony\Component\HttpFoundation\Response;
use Symfony\Component\Routing\Annotation\Route;

class ComputationController extends AbstractController
{

    private ApiService $apiService;
    public function __construct(ApiService $apiService)
    {
        $this->apiService = $apiService;
    }

    #[Route('/computations', name: 'computations')]
    public function index(ComputationRepository $computationRepository, Request $request, PaginatorInterface $paginator) : Response
    {
        if(!$this->getUser()){
            return $this->redirectToRoute('index');
        }

        $query = $computationRepository->findAll();

        $computations = $paginator->paginate(
            $query,
            $request->query->getInt('page', 1),
            10
        );

        return $this->render('computation/index.html.twig', [
            'computations' => $computations,
        ]);
    }

    #[Route('/api/computations/random', name: 'start_random', methods: ['GET'])]
    public function startRandom(EntityManagerInterface $entityManager) : JsonResponse{
        $computation = new Computation();
        $computation->setStartDate(new \DateTime());
        $computation->setEndDate(new \DateTime());
        $computation->setStatus("progress"); //status is whether in progess or stopped

        $types = $this->getTypes();
        $computationsActualResults = [];

        $entityManager->persist($computation);
        $entityManager->flush();

        foreach($types as $type){
            $computationsActualResults[$type] = new ComputationActualResult();
            $computationsActualResults[$type]->setType($type);
            $computationsActualResults[$type]->setComputation($computation);
            $entityManager->persist($computationsActualResults[$type]);
        }

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
        //Manage user permissions

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

        if($data['computationOver'] === true){
            $computation->setStatus("completed");
        }

        $entityManager->flush();

        return $this->json(['success' => true],201);
    }

    #[Route('/api/computations-actual-results', name: 'receive_actual_results', methods: ['POST'])]
    public function receiveActualResult(Request $request, EntityManagerInterface $entityManager, ComputationRepository $computationRepository): JsonResponse
    {
        $data = json_decode($request->getContent(), true);
        $computation = $computationRepository->findOneBy(['id' => $data['computationId']]);
        if(!$computation){
            return $this->json(['error' => 'Computation not found'], 404);
        }

        $type = $data['type'];

        $computationActualResults = $computation->getComputationActualResults();
        $result = null;

        foreach($computationActualResults as $computationActualResult){
            if($computationActualResult->getType() === $type){
                $result = $computationActualResult;
                break;
            }
        }

        if(!$result){
            return $this->json(['error' => 'Computation result not found'], 404);
        }

        $result->setDepths($data['depths'])
            ->setComputedValues($data['values'])
            ->setTime($data['time']);

        if($data['computationOver'] === true){
            $computation->setStatus("completed");
        }

        $entityManager->persist($computation);
        $entityManager->persist($result);
        $entityManager->flush();

        return $this->json([
            'success' => true,
        ]);
    }

    #[Route('computation/{id}/show', name: 'show_computation')]
    public function show(Computation $computation, EntityManagerInterface $entityManager, ComputationActualResultRepository $computationResultRepository)
    {
        //Manage user permissions

        $types = $this->getTypes();

        $graphData = [];

        foreach ($types as $type) {
            $result = $computationResultRepository->findOneBy(['computation' => $computation, 'type' => $type], ['id' => 'DESC']);
            if (!$result) continue;

            $dataset =  [
                'type' => $type,
                'label' => $this->getGraphLabel($type),
                'data' => array_map(fn($depth, $val) => ['x' => $depth, 'y' => $val], $result->getDepths(), $result->getComputedValues()),
                'borderColor' => $this->getGraphColor($type),
                'fill' => false,
                'tension' => 0.3,
                'time' => $this->getTimeString($type, $result->getTime()),
            ];

            $graphData[] = $dataset;

        }

        return $this->render('computation/show.html.twig', [
            'computation' => $computation,
            'datasets' => $graphData,
        ]);
    }

    #[Route('/api/computation/{id}/latest-results', name: 'latest_results', methods: ['GET'])]
    public function latestResults(
        Computation $computation,
        ComputationActualResultRepository $computationResultRepository
    ): JsonResponse
    {
        //Manage user permissions

        //all different types of data
        $types = $this->getTypes();

        $graphData = [];

        //fetching and processing the different graphs
        foreach ($types as $type) {
            $result = $computationResultRepository->findOneBy(['computation' => $computation, 'type' => $type], ['id' => 'DESC']);
            if (!$result) continue;

            $dataset =  [
                'type' => $type,
                'label' => $this->getGraphLabel($type),
                'data' => array_map(fn($depth, $val) => ['x' => $depth, 'y' => $val], $result->getDepths(), $result->getComputedValues()),
                'borderColor' => $this->getGraphColor($type),
                'fill' => false,
                'tension' => 0.3,
                'time' => $this->getTimeString($type, $result->getTime()),
            ];

            $graphData[] = $dataset;
        }

        return $this->json([
            'status' => $computation->getStatus(),
            'datasets' => $graphData,
        ]);
    }

    #[Route('computation/{id}/stop', name: 'stop_computation')]
    public function delete(Computation $computation, EntityManagerInterface $em)
    {
        //Manage user permissions

        if($computation->getStatus() !== "completed" && $computation->getStatus()!=="stopped" ){
            //call to API route /api/computing/cancel, computationId set via post
            $result = $this->apiService->stopComputing($computation->getId());
            if($result->getStatusCode() === 200){
                $computation->setStatus("stopped");
                $em->persist($computation);
                $em->flush();
                $this->addFlash('success', 'Computation was stopped successfully');
                return $this->redirectToRoute('show_computation', ['id' => $computation->getId()]);
            }
        }

        $this->addFlash('error', 'Computation could not be stopped');
        return $this->redirectToRoute('show_computation', ['id' => $computation->getId()]);
    }



    /**
     * @param $type
     * @return string
     * Method that returns a specified color for each types
     * default color #ccc (for unrecognized types)
     */
    public function getGraphColor($type): string
    {
        return match ($type) {
            "temperature_potential" => "#FF0000",
            "moisture_potential" => "#800080",
            "moisture_content" => "#0000FF",
            "total_chloride" => "#78f542",
            "free_chloride" => "#358014",
            "ph" => "#00A6CB",
            default => "#ccc",
        };
    }

    public function getGraphLabel($type): string
    {
        return match ($type) {
            "temperature_potential" => "Temperature Potential [°C]",
            "moisture_potential" => "Moisture Potential [P/Ps]",
            "moisture_content" => "Moisture Content [kg/m3]",
            "total_chloride" => "Total Chloride Ion Content [kg/m3]",
            "free_chloride" => "Free Chloride Ion Content [kg/m3]",
            "ph" => "PH",
            default => "unrecognized type",
        };
    }

    public function getTimeString($type, $time): string
    {
        $timeString = match ($type) {
            "temperature_potential" => "Temperature Potential Distribution at",
            "moisture_potential" => "Moisture Potential Distribution at",
            "moisture_content" => "Moisture Content Distribution at",
            "total_chloride" => "Total Chloride Ion Distribution at",
            "free_chloride" => "Free Chloride Ion Distribution at",
            "ph" => "PH Distribution at",
            default => "unrecognized type at",
        };

        return $timeString . " " . $time . " Days";
    }

    public function getTypes() : array
    {
        return [
            "temperature_potential",
            "moisture_potential",
            "moisture_content",
            "total_chloride",
            "free_chloride",
            "ph"
        ];
    }

}