<?php

namespace App\Entity;

use App\Repository\AggregateTypeRepository;
use Doctrine\ORM\Mapping as ORM;

#[ORM\Entity(repositoryClass: AggregateTypeRepository::class)]
class AggregateType
{
    #[ORM\Id]
    #[ORM\GeneratedValue]
    #[ORM\Column]
    private ?int $id = null;

    #[ORM\Column]
    private ?float $heatCapacity = null;

    #[ORM\Column(length: 255)]
    private ?string $name = null;

    #[ORM\Column]
    private ?float $aggregateDensity = null;

    public function getId(): ?int
    {
        return $this->id;
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

    public function getName(): ?string
    {
        return $this->name;
    }

    public function setName(string $name): static
    {
        $this->name = $name;

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
}
