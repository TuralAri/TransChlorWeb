<?php

namespace App\Entity;

use App\Repository\MaterialRepository;
use Doctrine\DBAL\Types\Types;
use Doctrine\ORM\Mapping as ORM;

#[ORM\Entity(repositoryClass: MaterialRepository::class)]
class Material
{
    #[ORM\Id]
    #[ORM\GeneratedValue]
    #[ORM\Column]
    private ?int $id = null;

    #[ORM\Column]
    private ?float $freshConcreteDensity = null;

    #[ORM\Column]
    private ?float $cementContent = null;

    #[ORM\Column]
    private ?float $saturatedWaterContent = null;

    #[ORM\Column]
    private ?float $airContent = null;

    #[ORM\Column]
    private ?float $ec = null;

    #[ORM\Column]
    private ?float $concreteAge = null;

    #[ORM\Column]
    private ?float $hydrationRate = null;

    #[ORM\ManyToOne(inversedBy: 'materials')]
    #[ORM\JoinColumn(nullable: false)]
    private ?AggregateType $aggregateType = null;

    #[ORM\Column]
    private ?float $heatCapacity = null;

    #[ORM\Column]
    private ?float $surfaceHeatTransfer = null;

    #[ORM\Column]
    private ?float $cementDensity = null;

    #[ORM\Column]
    private ?float $d100Percent = null;

    #[ORM\Column]
    private ?float $aoDiffusion = null;

    #[ORM\Column]
    private ?float $hc = null;

    #[ORM\Column]
    private ?float $ed = null;

    #[ORM\Column]
    private ?float $toDiffusion = null;

    #[ORM\Column]
    private ?float $surfaceTransferCoefficient = null;

    #[ORM\Column]
    private ?float $aoCapillarity = null;

    #[ORM\Column]
    private ?float $tc = null;

    #[ORM\Column]
    private ?float $dclTo = null;

    #[ORM\Column]
    private ?float $alphaDiffusion = null;

    #[ORM\Column]
    private ?float $toChlorideDiffusion = null;

    #[ORM\Column]
    private ?float $retardationCoefficient = null;

    #[ORM\Column]
    private ?float $limitWaterContent = null;

    #[ORM\Column]
    private ?float $adsorptionFa = null;

    #[ORM\Column]
    private ?float $alphaOh = null;

    #[ORM\Column]
    private ?float $eb = null;

    #[ORM\Column]
    private ?float $toAdsorption = null;

    #[ORM\Column(length: 255)]
    private ?string $CementType = null;

    #[ORM\Column]
    private ?float $aggregateContent = null;

    #[ORM\Column]
    private ?float $aggregateDensity = null;

    #[ORM\ManyToOne]
    #[ORM\JoinColumn(nullable: false)]
    private ?Permeability $permeability = null;

    #[ORM\Column]
    private ?bool $dclToValueBasedOnEc = null;

    #[ORM\ManyToOne(inversedBy: 'materials')]
    #[ORM\JoinColumn(nullable: false)]
    private ?User $user = null;

    #[ORM\Column(length: 255)]
    private ?string $name = null;

    #[ORM\Column(type: Types::TEXT)]
    private ?string $comment = null;

    public function getId(): ?int
    {
        return $this->id;
    }

    public function getFreshConcreteDensity(): ?float
    {
        return $this->freshConcreteDensity;
    }

    public function setFreshConcreteDensity(float $freshConcreteDensity): static
    {
        $this->freshConcreteDensity = $freshConcreteDensity;

        return $this;
    }

    public function getCementContent(): ?float
    {
        return $this->cementContent;
    }

    public function setCementContent(float $cementContent): static
    {
        $this->cementContent = $cementContent;

        return $this;
    }

    public function getSaturatedWaterContent(): ?float
    {
        return $this->saturatedWaterContent;
    }

    public function setSaturatedWaterContent(float $saturatedWaterContent): static
    {
        $this->saturatedWaterContent = $saturatedWaterContent;

        return $this;
    }

    public function getAirContent(): ?float
    {
        return $this->airContent;
    }

    public function setAirContent(float $airContent): static
    {
        $this->airContent = $airContent;

        return $this;
    }

    public function getEc(): ?float
    {
        return $this->ec;
    }

    public function setEc(float $ec): static
    {
        $this->ec = $ec;

        return $this;
    }

    public function getConcreteAge(): ?float
    {
        return $this->concreteAge;
    }

    public function setConcreteAge(float $concreteAge): static
    {
        $this->concreteAge = $concreteAge;

        return $this;
    }

    public function getHydrationRate(): ?float
    {
        return $this->hydrationRate;
    }

    public function setHydrationRate(float $hydrationRate): static
    {
        $this->hydrationRate = $hydrationRate;

        return $this;
    }

    public function getAggregateType(): ?AggregateType
    {
        return $this->aggregateType;
    }

    public function setAggregateType(?AggregateType $aggregateType): static
    {
        $this->aggregateType = $aggregateType;

        return $this;
    }

    public function getHeatCapacity(): ?float
    {
        return $this->heatCapacity;
    }

    public function setHeatCapacity(float $heatCapacity): static
    {
        $this->heatCapacity = $heatCapacity;

        return $this;
    }

    public function getSurfaceHeatTransfer(): ?float
    {
        return $this->surfaceHeatTransfer;
    }

    public function setSurfaceHeatTransfer(float $surfaceHeatTransfer): static
    {
        $this->surfaceHeatTransfer = $surfaceHeatTransfer;

        return $this;
    }

    public function getCementDensity(): ?float
    {
        return $this->cementDensity;
    }

    public function setCementDensity(float $cementDensity): static
    {
        $this->cementDensity = $cementDensity;

        return $this;
    }

    public function getD100Percent(): ?float
    {
        return $this->d100Percent;
    }

    public function setD100Percent(float $d100Percent): static
    {
        $this->d100Percent = $d100Percent;

        return $this;
    }

    public function getAoDiffusion(): ?float
    {
        return $this->aoDiffusion;
    }

    public function setAoDiffusion(float $aoDiffusion): static
    {
        $this->aoDiffusion = $aoDiffusion;

        return $this;
    }

    public function getHc(): ?float
    {
        return $this->hc;
    }

    public function setHc(float $hc): static
    {
        $this->hc = $hc;

        return $this;
    }

    public function getEd(): ?float
    {
        return $this->ed;
    }

    public function setEd(float $ed): static
    {
        $this->ed = $ed;

        return $this;
    }

    public function getToDiffusion(): ?float
    {
        return $this->toDiffusion;
    }

    public function setToDiffusion(float $toDiffusion): static
    {
        $this->toDiffusion = $toDiffusion;

        return $this;
    }

    public function getSurfaceTransferCoefficient(): ?float
    {
        return $this->surfaceTransferCoefficient;
    }

    public function setSurfaceTransferCoefficient(float $surfaceTransferCoefficient): static
    {
        $this->surfaceTransferCoefficient = $surfaceTransferCoefficient;

        return $this;
    }

    public function getAoCapillarity(): ?float
    {
        return $this->aoCapillarity;
    }

    public function setAoCapillarity(float $aoCapillarity): static
    {
        $this->aoCapillarity = $aoCapillarity;

        return $this;
    }

    public function getTc(): ?float
    {
        return $this->tc;
    }

    public function setTc(float $tc): static
    {
        $this->tc = $tc;

        return $this;
    }

    public function getDclTo(): ?float
    {
        return $this->dclTo;
    }

    public function setDclTo(float $dclTo): static
    {
        $this->dclTo = $dclTo;

        return $this;
    }

    public function getAlphaDiffusion(): ?float
    {
        return $this->alphaDiffusion;
    }

    public function setAlphaDiffusion(float $alphaDiffusion): static
    {
        $this->alphaDiffusion = $alphaDiffusion;

        return $this;
    }

    public function getToChlorideDiffusion(): ?float
    {
        return $this->toChlorideDiffusion;
    }

    public function setToChlorideDiffusion(float $toChlorideDiffusion): static
    {
        $this->toChlorideDiffusion = $toChlorideDiffusion;

        return $this;
    }

    public function getRetardationCoefficient(): ?float
    {
        return $this->retardationCoefficient;
    }

    public function setRetardationCoefficient(float $retardationCoefficient): static
    {
        $this->retardationCoefficient = $retardationCoefficient;

        return $this;
    }

    public function getLimitWaterContent(): ?float
    {
        return $this->limitWaterContent;
    }

    public function setLimitWaterContent(float $limitWaterContent): static
    {
        $this->limitWaterContent = $limitWaterContent;

        return $this;
    }

    public function getAdsorptionFa(): ?float
    {
        return $this->adsorptionFa;
    }

    public function setAdsorptionFa(float $adsorptionFa): static
    {
        $this->adsorptionFa = $adsorptionFa;

        return $this;
    }

    public function getAlphaOh(): ?float
    {
        return $this->alphaOh;
    }

    public function setAlphaOh(float $alphaOh): static
    {
        $this->alphaOh = $alphaOh;

        return $this;
    }

    public function getEb(): ?float
    {
        return $this->eb;
    }

    public function setEb(float $eb): static
    {
        $this->eb = $eb;

        return $this;
    }

    public function getToAdsorption(): ?float
    {
        return $this->toAdsorption;
    }

    public function setToAdsorption(float $toAdsorption): static
    {
        $this->toAdsorption = $toAdsorption;

        return $this;
    }

    public function getCementType(): ?string
    {
        return $this->CementType;
    }

    public function setCementType(string $CementType): static
    {
        $this->CementType = $CementType;

        return $this;
    }

    public function getAggregateContent(): ?float
    {
        return $this->aggregateContent;
    }

    public function setAggregateContent(float $aggregateContent): static
    {
        $this->aggregateContent = $aggregateContent;

        return $this;
    }

    public function getAggregateDensity(): ?float
    {
        return $this->aggregateDensity;
    }

    public function setAggregateDensity(float $aggregateDensity): static
    {
        $this->aggregateDensity = $aggregateDensity;

        return $this;
    }

    public function getPermeability(): ?Permeability
    {
        return $this->permeability;
    }

    public function setPermeability(?Permeability $permeability): static
    {
        $this->permeability = $permeability;

        return $this;
    }

    public function isDclToValueBasedOnEc(): ?bool
    {
        return $this->dclToValueBasedOnEc;
    }

    public function setDclToValueBasedOnEc(bool $dclToValueBasedOnEc): static
    {
        $this->dclToValueBasedOnEc = $dclToValueBasedOnEc;

        return $this;
    }

    public function getUser(): ?User
    {
        return $this->user;
    }

    public function setUser(?User $user): static
    {
        $this->user = $user;

        return $this;
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
}
