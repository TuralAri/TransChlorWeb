<?php

namespace App\Entity;

use App\Repository\PermeabilityRepository;
use Doctrine\ORM\Mapping as ORM;

#[ORM\Entity(repositoryClass: PermeabilityRepository::class)]
class Permeability
{
    #[ORM\Id]
    #[ORM\GeneratedValue]
    #[ORM\Column]
    private ?int $id = null;

    #[ORM\Column]
    private ?float $d100Percent = null;

    #[ORM\Column]
    private ?float $dclTo = null;

    #[ORM\Column]
    private ?float $heatCapacity = null;

    #[ORM\Column]
    private ?float $surfaceTransferCoefficient = null;

    #[ORM\Column]
    private ?float $surfaceHeatTransfer = null;

    #[ORM\Column]
    private ?float $cementDensity = null;

    #[ORM\Column]
    private ?float $ec = null;

    #[ORM\Column]
    private ?float $freshConcreteDensity = null;

    #[ORM\Column]
    private ?float $hydrationRate = null;

    #[ORM\Column]
    private ?float $airContent = null;

    #[ORM\Column]
    private ?float $ed = null;

    #[ORM\Column]
    private ?float $toDiffusion = null;

    #[ORM\Column]
    private ?float $alphaDiffusion = null;

    #[ORM\Column]
    private ?float $toChlorideDiffusion = null;

    #[ORM\Column(length: 255)]
    private ?string $name = null;

    public function getId(): ?int
    {
        return $this->id;
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

    public function getDclTo(): ?float
    {
        return $this->dclTo;
    }

    public function setDclTo(float $dclTo): static
    {
        $this->dclTo = $dclTo;

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

    public function getSurfaceTransferCoefficient(): ?float
    {
        return $this->surfaceTransferCoefficient;
    }

    public function setSurfaceTransferCoefficient(float $surfaceTransferCoefficient): static
    {
        $this->surfaceTransferCoefficient = $surfaceTransferCoefficient;

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

    public function getEc(): ?float
    {
        return $this->ec;
    }

    public function setEc(float $ec): static
    {
        $this->ec = $ec;

        return $this;
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

    public function getHydrationRate(): ?float
    {
        return $this->hydrationRate;
    }

    public function setHydrationRate(float $hydrationRate): static
    {
        $this->hydrationRate = $hydrationRate;

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

    public function getName(): ?string
    {
        return $this->name;
    }

    public function setName(string $name): static
    {
        $this->name = $name;

        return $this;
    }
}
