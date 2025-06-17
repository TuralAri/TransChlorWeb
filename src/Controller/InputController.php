<?php

namespace App\Controller;

use App\Entity\Input;
use App\Entity\Material;
use App\Entity\ProbabilisticLawParams;
use App\Form\InputFormType;
use App\Form\ProbabilisticLawFormType;
use App\Repository\InputRepository;
use App\Repository\ProbabilisticLawParamsRepository;
use App\Service\ApiService;
use Doctrine\ORM\EntityManagerInterface;
use http\Exception\RuntimeException;
use Knp\Component\Pager\PaginatorInterface;
use Symfony\Bundle\FrameworkBundle\Controller\AbstractController;
use Symfony\Component\DependencyInjection\ParameterBag\ParameterBagInterface;
use Symfony\Component\Form\FormFactoryInterface;
use Symfony\Component\HttpFoundation\BinaryFileResponse;
use Symfony\Component\HttpFoundation\Request;
use Symfony\Component\HttpFoundation\Response;
use Symfony\Component\HttpFoundation\ResponseHeaderBag;
use Symfony\Component\Routing\Attribute\Route;
use Symfony\Contracts\Translation\TranslatorInterface;

class InputController extends AbstractController
{
    private ApiService $apiService;
    public function __construct(ApiService $apiService)
    {
        $this->apiService = $apiService;
    }

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
        $form = $this->createForm(InputFormType::class, $input, [
            'attr' => ['id' => 'input_form'],
            'user' => $this->getUser(),
            'em' => $entityManager,
        ]);
        $form->handleRequest($request);
        if ($form->isSubmitted() && $form->isValid()) {
            $input = $form->getData();

            $probabilisticDataRaw = $form->get('probabilisticData')->getData();

            if ($probabilisticDataRaw) {
                $probabilisticData = json_decode($probabilisticDataRaw, true);

                $transports = [
                    'waterVaporTransport' => 'isWaterVaporTransportActivated',
                    'capillarityTransport' => 'isCapillarityTransportActivated',
                    'ionicTransport' => 'isIonicTransportActivated',
                    'carbonation' => 'isCarbonatationActivated',
                ];

                foreach ($probabilisticData as $probabilisticItem) {
                    foreach ($transports as $key => $activationMethod) {
                        if (method_exists($input, $activationMethod) && $input->$activationMethod()) {
                            if (isset($probabilisticItem[$key])) {
                                $transport = new ProbabilisticLawParams();
                                $this->setProbabilisticData($probabilisticItem[$key], $transport, $entityManager);
                                $input->addProbabilisticParam($transport);
                                $entityManager->persist($transport);
                            }
                        }
                    }
                }
            }

            $input->setUser($user);

            $entityManager->persist($input);
            $entityManager->flush();
            return $this->redirectToRoute('inputs');
        }

        return $this->render('inputs/add.html.twig', [
            'form' => $form->createView(),
        ]);
    }

    public function setProbabilisticData($array, ProbabilisticLawParams $probabilisticLawParams, EntityManagerInterface $entityManager) : void
    {
        $material = $entityManager->getRepository(Material::class)->find($array['material']);
        $probabilisticLawParams->setMaterial($material);
        $probabilisticLawParams->setType($array['type']);
        $probabilisticLawParams->setLawType($array['lawType']);
        $probabilisticLawParams->setMeanValue($array['meanValue']);
        $probabilisticLawParams->setStandardDeviation($array['standardDeviation']);
        if($array['lawType'] == 2){ //logonormal law
            $probabilisticLawParams->setLambda($array['lambda']);
            $probabilisticLawParams->setKsi($array['ksi']);
        }
        $probabilisticLawParams->setPMinus($array['pMinus']);
        $probabilisticLawParams->setPPlus($array['pPlus']);
        $probabilisticLawParams->setX1($array['x1']);
        $probabilisticLawParams->setX2($array['x2']);
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

        return $this->render('inputs/edit.html.twig', [
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

    public function toRelativePath(string $absolutePath, ParameterBagInterface $params): string
    {
        $uploadDir = rtrim($params->get('relative_upload_directory'), '/');
        $projectRoot = realpath(__DIR__ . '/../../');
        $basePath = $projectRoot . '/' . ltrim($uploadDir, '/');

        //In case you're coding on Windows
        $absolutePath = str_replace('\\', '/', $absolutePath);
        $basePath = str_replace('\\', '/', $basePath);

        return ltrim(str_replace($basePath, '', $absolutePath), '/');
    }

    #[Route('/inputs/{id}/generate', name: 'generate_input')]
    public function generateInput(Input $input, ProbabilisticLawParamsRepository $lawParamsRepository, ParameterBagInterface $bag): Response
    {

        $filePath = $this->writeInputFile($input, $lawParamsRepository, $bag);

        $response = new BinaryFileResponse($filePath);
        $response->setContentDisposition(
            ResponseHeaderBag::DISPOSITION_ATTACHMENT
        );

        return $response;
    }

    public function writeInputFile(Input $input, ProbabilisticLawParamsRepository $lawParamsRepository, ParameterBagInterface $bag) : String
    {
        $materials = $input->getMaterial();

        $expositionDirectory = $this->getParameter('upload_directory') . '/Ressources/Exposition/';
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

        fwrite($handle, $input->getMeshType() . "\n"); //ChoixRep
        switch ($input->getMeshType()) {
            case 1:
                break; //Nothing
            case 2: //proportional deviation
            case 3: //exponential deviation
                fwrite($handle, $input->getEdgeElementLength() . "\n");
                break;
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
        fwrite($handle, $input->getAoDiffusion() . "\n"); //aa
        fwrite($handle, $input->getHc() . "\n"); //hc
        fwrite($handle, $input->getAoCapillarity() . "\n"); //ab
        fwrite($handle, $input->getTc() . "\n"); //tc
        switch ($input->getCapillarityTreatment()) { //ImpHydr
            case 1:
                fwrite($handle, "False" . "\n");
                break;
            case 2:
                fwrite($handle, "True" . "\n");
                break;
        }
        fwrite($handle, $input->getLimitWaterContent() . "\n"); //H_Snap
        fwrite($handle, $input->getDelayCoefficient() . "\n"); //Retard
        fwrite($handle, $input->getAlphaOh() . "\n"); //aOH
        fwrite($handle, $input->getEb() . "\n"); //EbG
        fwrite($handle, $input->getToAdsorption() . "\n"); //toG
        fwrite($handle, $input->getAdsorptionFa() . "\n"); //faG
        fwrite($handle, $input->getHeatCapacity() . "\n"); //capCal
        fwrite($handle, $input->getLeftEdgeCO2() . "\n"); //GyCO2
        fwrite($handle, $input->getRightEdgeCO2() . "\n"); //DyCO2
        if($input->getExposureFile1()->getId() == $input->getExposureFile2()->getId()){ //Number of exposition files
            fwrite($handle, "1" . "\n");
        }else{
            fwrite($handle, "1" . "\n");
        }

        $leftExpostionLink = $expositionDirectory . $input->getExposureFile1()->getExposureSerie()->getId() . '/' . $input->getExposureFile1()->getFilename();
        $rightExpostionLink = $expositionDirectory . $input->getExposureFile2()->getExposureSerie()->getId() . '/' . $input->getExposureFile2()->getFilename();

        if($bag->get('using_docker') === 'true' ){
            $leftExpostionLink = $this->toRelativePath($leftExpostionLink, $bag);
            $rightExpostionLink = $this->toRelativePath($rightExpostionLink, $bag);
        }

        fwrite($handle, $leftExpostionLink . "\n");
        fwrite($handle, $rightExpostionLink . "\n");

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
            switch ($material->getCementType()){
                case 1:
                    fwrite($handle, "0.9" . "\n");
                    fwrite($handle, "1.1" . "\n");
                    break;
                case 2:
                    fwrite($handle, "1.0" . "\n");
                    fwrite($handle, "1.0" . "\n");
                    break;
                case 3:
                    fwrite($handle, "0.85" . "\n");
                    fwrite($handle, "1.15" . "\n");
                    break;
                case 4:
                    fwrite($handle, "0.6" . "\n");
                    fwrite($handle, "1.5" . "\n");
                    break;

            }
            //
            fwrite($handle, $material->getEd() . "\n");//énergie d'activation pour la vapeur d'eau (température)
            fwrite($handle, $material->getToDiffusion() . "\n");//température de référence pour l'énergie d'activation précédente
            fwrite($handle, $material->getAlphaDiffusion() . "\n");//énergie d'activation pour l'entraînement des ions cl- par l'eau (température)
            fwrite($handle, $material->getToChlorideDiffusion() . "\n");//température de référence pour l'énergie d'activation précédente
            fwrite($handle, $material->getAggregateDensity() . "\n");//masse volumique des granulats
            fwrite($handle, $material->getCementDensity() . "\n");//masse volumique du ciment
            //
            $transports = [
                'waterVaporTransport' => 'isWaterVaporTransportActivated',
                'capillarityTransport' => 'isCapillarityTransportActivated',
                'ionicTransport' => 'isIonicTransportActivated',
                'carbonation' => 'isCarbonatationActivated',
            ];

            foreach ($transports as $type => $activationMethod) {
                if (method_exists($input, $activationMethod) && $input->$activationMethod()) {
                    $lawParam = $lawParamsRepository->findOneByMaterialInputAndType($material, $input, $type);
                    $this->writeLawParams($lawParam, $handle);
                }else{
                    for ($i = 0; $i < 5; $i++) {
                        fwrite($handle, "0" . "\n");
                    }
                }
            }
            //
            fwrite($handle, $material->getEc() . "\n");//eau sur ciment pour le calcul Dcap // CORRESPOND AU E/C VIRTUEL
            //
        }

        $this->writeInitialCondition($input->getThermalTransport(), $handle);
        $this->writeInitialCondition($input->getWaterTransport(), $handle);
        $this->writeInitialCondition($input->getIonicTransport(), $handle);
        //

        fclose($handle);

        return $filePath;
    }

    public function writeLawParams(ProbabilisticLawParams $lawParams, $handle) : void
    {
        //Normal law case
        if($lawParams->getLawType() == 1){
            fwrite($handle, "1" . "\n");
            fwrite($handle, ($lawParams->getMeanValue() - $lawParams->getStandardDeviation()) / $lawParams->getMeanValue() . "\n");
            fwrite($handle, ($lawParams->getMeanValue() + $lawParams->getStandardDeviation()) / $lawParams->getMeanValue() . "\n");
            fwrite($handle, "0.5" . "\n");
            fwrite($handle, "0.5" . "\n");
        }
        //logonormal law case
        else if($lawParams->getLawType() == 2){
            fwrite($handle, "2" . "\n");
            fwrite($handle, ((exp($lawParams->getLambda() - $lawParams->getKsi())) / $lawParams->getMeanValue()) . "\n");
            fwrite($handle, ((exp($lawParams->getLambda() + $lawParams->getKsi())) / $lawParams->getMeanValue()) . "\n");
            $sm = exp($lawParams->getLambda()) * (1 - exp(-$lawParams->getKsi()));
            $sp = exp($lawParams->getLambda()) * (exp($lawParams->getKsi()) - 1);
            fwrite($handle, ($sp / ($sp + $sm)) . "\n");
            fwrite($handle, ($sm / ($sp + $sm)) . "\n");
        }else{
            throw new RuntimeException("Unknown law type: " . $lawParams->getLawType());
        }
    }

    #[Route('/inputs/{id}/compute', name: 'launch_computation', methods: ['POST'])]
    public function compute(Input $input, Request $request, TranslatorInterface $translator, ProbabilisticLawParamsRepository $lawParamsRepository, ParameterBagInterface $bag) : Response
    {
        $filepath = $this->writeInputFile($input, $lawParamsRepository, $bag);

        if($bag->get('using_docker') === 'true' ){
            $filepath = $this->toRelativePath($filepath, $bag);
        }

        $data = json_decode($request->getContent(), true);
        $response = $this->forward('App\Controller\ComputationController::start1D', [
           'outfile' => $filepath,
            'data' => json_encode($data),
            'length' => $input->getWallThickness()
        ]);
        $response = json_decode($response->getContent(), true);

        return $this->redirectToRoute("show_computation", [
            'id' => $response['computationId'],
        ]);
    }

    public function writeInitialCondition(Array $data, $handle) : void {
        fwrite($handle,count($data) . "\n");
        foreach($data as $val){
            fwrite($handle, $val['x'] . "\n");
            fwrite($handle, $val['value'] . "\n");
        }
    }

    #[Route('/material/{id}/form/{type}', name: 'material_probabilistic_law_form')]
    public function getMaterialForm(Material $material, string $type, FormFactoryInterface $formFactory) : Response
    {
        if(!$material){
            throw $this->createNotFoundException('Material not found');
        }

        $probabilisticLawParams = new ProbabilisticLawParams();
        $probabilisticLawParams->setMaterial($material);
        $probabilisticLawParams->setType($type);

        switch($type){
            case "waterVaporTransport":
                $probabilisticLawParams->setMeanValue($material->getD100Percent());
                $probabilisticLawParams->setStandardDeviation($material->getD100Percent() / 1.36);
                break;
            case "capillarityTransport":
                $ec = $material->getEc();
                $a = 0.0000625 * ($ec * $ec) - 0.000104 * $ec + 0.00003;
                $b = -0.015547 * ($ec * $ec) + 0.021655 * $ec - 0.005652;
                $probabilisticLawParams->setMeanValue($a * 100 + $b);
                $probabilisticLawParams->setStandardDeviation(0.00005962);
                break;
            case "ionicTransport":
                $probabilisticLawParams->setMeanValue($material->getDclTo());
                $probabilisticLawParams->setStandardDeviation(0.000005772);
                break;
            case "carbonation":
                $ec = $material->getEc();
                $cementDensity = $material->getCementDensity();
                error_log("CEMENT TEST" . $cementDensity);

                $numerator = $cementDensity * ($ec - 0.3) * (1 - 0.7);
                $denominator = 1000 * (1 + $cementDensity * $ec / 1000);
                $expression = $numerator / $denominator;

                $meanValue = 2.8 * ($expression * $expression);

                $probabilisticLawParams->setMeanValue($meanValue);
                $probabilisticLawParams->setStandardDeviation(0.000005772);
                break;
        }

        $this->setLawParameters($probabilisticLawParams);

//        $form = $this->createForm(ProbabilisticLawFormType::class, $probabilisticLawParams);

        $formName = sprintf('probabilistic_%d_%s', $material->getId(), $type);

        $form = $formFactory->createNamed(
            $formName,
            ProbabilisticLawFormType::class,
            $probabilisticLawParams
        );

        $html = $this->render('inputs/form.html.twig', [
            'form' => $form->createView(),
            'materialId' => $material->getId(),
            'type' => $type,
        ]);

        return $this->json([
           'success' => true,
//           'form' => $this->renderView('inputs/form.html.twig', [
//               'form' => $form->createView(),
//           ])
            'form' => $html->getContent(),
            'materialData' => [
                'ec' => $material->getEc(),
                'd100' => $material->getD100Percent(),
                'dcl' => $material->getDclTo(),
                'cementDensity' => $material->getCementDensity(),
            ],
        ]);
    }

    function setLawParameters(ProbabilisticLawParams $probabilisticLawParams) : void
    {
        $probabilisticLawParams->setLambda(
            log(
                pow($probabilisticLawParams->getMeanValue(), 2) /
                sqrt(pow($probabilisticLawParams->getMeanValue(), 2) + pow($probabilisticLawParams->getStandardDeviation(), 2))
            )
        );
        $probabilisticLawParams->setKsi(
            sqrt(log(
                pow($probabilisticLawParams->getStandardDeviation(), 2) /
                pow($probabilisticLawParams->getMeanValue(), 2) + 1
            ))
        );
        $sm = exp($probabilisticLawParams->getLambda()) * (1- exp(-$probabilisticLawParams->getKsi()));
        $sp = exp($probabilisticLawParams->getLambda()) * (exp($probabilisticLawParams->getKsi())-1);
        $probabilisticLawParams->setPMinus($sp / ($sp + $sm));
        $probabilisticLawParams->setPPlus($sm / ($sp + $sm));

        $probabilisticLawParams->setX1(exp($probabilisticLawParams->getLambda() - $probabilisticLawParams->getKsi()));
        $probabilisticLawParams->setX2(exp($probabilisticLawParams->getLambda() + $probabilisticLawParams->getKsi()));
    }

}