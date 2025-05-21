<?php

namespace App\Entity;

use App\Repository\InputRepository;
use Doctrine\Common\Collections\ArrayCollection;
use Doctrine\Common\Collections\Collection;
use Doctrine\DBAL\Types\Types;
use Doctrine\ORM\Mapping as ORM;

#[ORM\Entity(repositoryClass: InputRepository::class)]
class Input
{
    #[ORM\Id]
    #[ORM\GeneratedValue]
    #[ORM\Column]
    private ?int $id = null;

    #[ORM\Column(length: 255)]
    private ?string $name = null;

    #[ORM\Column(type: Types::TEXT)]
    private ?string $comment = null;

    #[ORM\ManyToOne]
    #[ORM\JoinColumn(nullable: false)]
    private ?Exposure $exposureFile1 = null;

    #[ORM\ManyToOne]
    #[ORM\JoinColumn(nullable: false)]
    private ?Exposure $exposureFile2 = null;

    /**
     * @var Collection<int, Material>
     */
    #[ORM\ManyToMany(targetEntity: Material::class)]
    private Collection $material;

    #[ORM\Column]
    private ?int $saveTimeTemperature = null;

    #[ORM\Column]
    private ?int $saveTimeRelativeHumidity = null;

    #[ORM\Column]
    private ?int $saveTimeWaterContent = null;

    #[ORM\Column]
    private ?int $saveTimePh = null;

    #[ORM\Column]
    private ?int $saveTimeFreeChlorures = null;

    #[ORM\Column]
    private ?int $saveTimeTotalChlorures = null;

    #[ORM\Column]
    private ?float $maxComputingTime = null;

    #[ORM\Column]
    private ?int $computingTimeStep = null;

    #[ORM\Column]
    private ?int $wallThickness = null;

    #[ORM\Column]
    private ?int $elementsNumber = null;

    #[ORM\Column]
    private ?int $meshType = null;

    #[ORM\Column]
    private ?int $resultsDisplayTime = null;

    #[ORM\Column]
    private ?int $capillarityTreatment = null;

    #[ORM\Column]
    private ?float $leftEdgeCO2 = null;

    #[ORM\Column]
    private ?float $rightEdgeCO2 = null;

    #[ORM\Column]
    private ?int $leftEdgeCO2Choice = null;

    #[ORM\Column]
    private ?int $rightEdgeCO2Choice = null;

    #[ORM\Column]
    private array $thermalTransport = [];

    #[ORM\Column]
    private array $waterTransport = [];

    #[ORM\Column]
    private array $IonicTransport = [];

    #[ORM\Column]
    private ?bool $isWaterVaporTransportActivated = null;//$waterTransportProbabilisticActivated

    #[ORM\Column]
    private ?bool $isCapillarityTransportActivated = null;

    #[ORM\Column]
    private ?bool $isIonicTransportActivated = null;

    #[ORM\Column]
    private ?bool $isCarbonatationActivated = null;

    #[ORM\OneToOne(cascade: ['persist', 'remove'])]
    private ?ProbabilisticLawParams $vaporWaterTransport = null;

    #[ORM\OneToOne(cascade: ['persist', 'remove'])]
    private ?ProbabilisticLawParams $liquidWaterTransportCapillarity = null;

    #[ORM\OneToOne(cascade: ['persist', 'remove'])]
    private ?ProbabilisticLawParams $chlorideIonicTransport = null;

    #[ORM\OneToOne(cascade: ['persist', 'remove'])]
    private ?ProbabilisticLawParams $carbonation = null;

    public function __construct()
    {
        $this->material = new ArrayCollection();
    }

    public function getId(): ?int
    {
        return $this->id;
    }

    public function getName(): ?string
    {
        return $this->name;
    }

    public function setName(string $name): static
    {
        $this->name = $name;

        return $this;
    }

    public function getComment(): ?string
    {
        return $this->comment;
    }

    public function setComment(string $comment): static
    {
        $this->comment = $comment;

        return $this;
    }

    public function getExposureFile1(): ?Exposure
    {
        return $this->exposureFile1;
    }

    public function setExposureFile1(?Exposure $exposureFile1): static
    {
        $this->exposureFile1 = $exposureFile1;

        return $this;
    }

    public function getExposureFile2(): ?Exposure
    {
        return $this->exposureFile2;
    }

    public function setExposureFile2(?Exposure $exposureFile2): static
    {
        $this->exposureFile2 = $exposureFile2;

        return $this;
    }

    /**
     * @return Collection<int, Material>
     */
    public function getMaterial(): Collection
    {
        return $this->material;
    }

    public function addMaterial(Material $material): static
    {
        if (!$this->material->contains($material)) {
            $this->material->add($material);
        }

        return $this;
    }

    public function removeMaterial(Material $material): static
    {
        $this->material->removeElement($material);

        return $this;
    }

    public function getSaveTimeTemperature(): ?int
    {
        return $this->saveTimeTemperature;
    }

    public function setSaveTimeTemperature(int $saveTimeTemperature): static
    {
        $this->saveTimeTemperature = $saveTimeTemperature;

        return $this;
    }

    public function getSaveTimeRelativeHumidity(): ?int
    {
        return $this->saveTimeRelativeHumidity;
    }

    public function setSaveTimeRelativeHumidity(int $saveTimeRelativeHumidity): static
    {
        $this->saveTimeRelativeHumidity = $saveTimeRelativeHumidity;

        return $this;
    }

    public function getSaveTimeWaterContent(): ?int
    {
        return $this->saveTimeWaterContent;
    }

    public function setSaveTimeWaterContent(int $saveTimeWaterContent): static
    {
        $this->saveTimeWaterContent = $saveTimeWaterContent;

        return $this;
    }

    public function getSaveTimePh(): ?int
    {
        return $this->saveTimePh;
    }

    public function setSaveTimePh(int $saveTimePh): static
    {
        $this->saveTimePh = $saveTimePh;

        return $this;
    }

    public function getSaveTimeFreeChlorures(): ?int
    {
        return $this->saveTimeFreeChlorures;
    }

    public function setSaveTimeFreeChlorures(int $saveTimeFreeChlorures): static
    {
        $this->saveTimeFreeChlorures = $saveTimeFreeChlorures;

        return $this;
    }

    public function getSaveTimeTotalChlorures(): ?int
    {
        return $this->saveTimeTotalChlorures;
    }

    public function setSaveTimeTotalChlorures(int $saveTimeTotalChlorures): static
    {
        $this->saveTimeTotalChlorures = $saveTimeTotalChlorures;

        return $this;
    }

    public function getMaxComputingTime(): ?float
    {
        return $this->maxComputingTime;
    }

    public function setMaxComputingTime(float $maxComputingTime): static
    {
        $this->maxComputingTime = $maxComputingTime;

        return $this;
    }

    public function getComputingTimeStep(): ?int
    {
        return $this->computingTimeStep;
    }

    public function setComputingTimeStep(int $computingTimeStep): static
    {
        $this->computingTimeStep = $computingTimeStep;

        return $this;
    }

    public function getWallThickness(): ?int
    {
        return $this->wallThickness;
    }

    public function setWallThickness(int $wallThickness): static
    {
        $this->wallThickness = $wallThickness;

        return $this;
    }

    public function getElementsNumber(): ?int
    {
        return $this->elementsNumber;
    }

    public function setElementsNumber(int $elementsNumber): static
    {
        $this->elementsNumber = $elementsNumber;

        return $this;
    }

    public function getMeshType(): ?int
    {
        return $this->meshType;
    }

    public function setMeshType(int $meshType): static
    {
        $this->meshType = $meshType;

        return $this;
    }

    public function getResultsDisplayTime(): ?int
    {
        return $this->resultsDisplayTime;
    }

    public function setResultsDisplayTime(int $resultsDisplayTime): static
    {
        $this->resultsDisplayTime = $resultsDisplayTime;

        return $this;
    }

    public function getCapillarityTreatment(): ?int
    {
        return $this->capillarityTreatment;
    }

    public function setCapillarityTreatment(int $capillarityTreatment): static
    {
        $this->capillarityTreatment = $capillarityTreatment;

        return $this;
    }

    public function getLeftEdgeCO2(): ?float
    {
        return $this->leftEdgeCO2;
    }

    public function setLeftEdgeCO2(float $leftEdgeCO2): static
    {
        $this->leftEdgeCO2 = $leftEdgeCO2;

        return $this;
    }

    public function getRightEdgeCO2(): ?float
    {
        return $this->rightEdgeCO2;
    }

    public function setRightEdgeCO2(float $rightEdgeCO2): static
    {
        $this->rightEdgeCO2 = $rightEdgeCO2;

        return $this;
    }

    public function getLeftEdgeCO2Choice(): ?int
    {
        return $this->leftEdgeCO2Choice;
    }

    public function setLeftEdgeCO2Choice(int $leftEdgeCO2Choice): static
    {
        $this->leftEdgeCO2Choice = $leftEdgeCO2Choice;

        return $this;
    }

    public function getRightEdgeCO2Choice(): ?int
    {
        return $this->rightEdgeCO2Choice;
    }

    public function setRightEdgeCO2Choice(int $rightEdgeCO2Choice): static
    {
        $this->rightEdgeCO2Choice = $rightEdgeCO2Choice;

        return $this;
    }

    public function getThermalTransport(): array
    {
        return $this->thermalTransport;
    }

    public function setThermalTransport(array $thermalTransport): static
    {
        $this->thermalTransport = $thermalTransport;

        return $this;
    }

    public function getWaterTransport(): array
    {
        return $this->waterTransport;
    }

    public function setWaterTransport(array $waterTransport): static
    {
        $this->waterTransport = $waterTransport;

        return $this;
    }

    public function getIonicTransport(): array
    {
        return $this->IonicTransport;
    }

    public function setIonicTransport(array $IonicTransport): static
    {
        $this->IonicTransport = $IonicTransport;

        return $this;
    }

    public function isWaterVaporTransportActivated(): ?bool
    {
        return $this->isWaterVaporTransportActivated;
    }

    public function setWaterVaporTransportActivated(bool $isWaterVaporTransportActivated): static
    {
        $this->isWaterVaporTransportActivated = $isWaterVaporTransportActivated;

        return $this;
    }

    public function isCapillarityTransportActivated(): ?bool
    {
        return $this->isCapillarityTransportActivated;
    }

    public function setCapillarityTransportActivated(bool $isCapillarityTransportActivated): static
    {
        $this->isCapillarityTransportActivated = $isCapillarityTransportActivated;

        return $this;
    }

    public function isIonicTransportActivated(): ?bool
    {
        return $this->isIonicTransportActivated;
    }

    public function setIonicTransportActivated(bool $isIonicTransportActivated): static
    {
        $this->isIonicTransportActivated = $isIonicTransportActivated;

        return $this;
    }

    public function isCarbonatationActivated(): ?bool
    {
        return $this->isCarbonatationActivated;
    }

    public function setCarbonatationActivated(bool $isCarbonatationActivated): static
    {
        $this->isCarbonatationActivated = $isCarbonatationActivated;

        return $this;
    }

    public function getVaporWaterTransport(): ?ProbabilisticLawParams
    {
        return $this->vaporWaterTransport;
    }

    public function setVaporWaterTransport(?ProbabilisticLawParams $vaporWaterTransport): static
    {
        $this->vaporWaterTransport = $vaporWaterTransport;

        return $this;
    }

    public function getLiquidWaterTransportCapillarity(): ?ProbabilisticLawParams
    {
        return $this->liquidWaterTransportCapillarity;
    }

    public function setLiquidWaterTransportCapillarity(?ProbabilisticLawParams $liquidWaterTransportCapillarity): static
    {
        $this->liquidWaterTransportCapillarity = $liquidWaterTransportCapillarity;

        return $this;
    }

    public function getChlorideIonicTransport(): ?ProbabilisticLawParams
    {
        return $this->chlorideIonicTransport;
    }

    public function setChlorideIonicTransport(?ProbabilisticLawParams $chlorideIonicTransport): static
    {
        $this->chlorideIonicTransport = $chlorideIonicTransport;

        return $this;
    }

    public function getCarbonation(): ?ProbabilisticLawParams
    {
        return $this->carbonation;
    }

    public function setCarbonation(?ProbabilisticLawParams $carbonation): static
    {
        $this->carbonation = $carbonation;

        return $this;
    }
}
