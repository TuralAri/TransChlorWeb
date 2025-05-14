<?php

namespace App\Entity;

use App\Repository\ComputationResultRepository;
use Doctrine\ORM\Mapping as ORM;

#[ORM\Entity(repositoryClass: ComputationResultRepository::class)]
class ComputationResult
{
    #[ORM\Id]
    #[ORM\GeneratedValue]
    #[ORM\Column]
    private ?int $id = null;

    #[ORM\Column]
    private ?int $time = null;

    #[ORM\Column]
    private array $computedValues = [];

    #[ORM\Column(length: 255)]
    private ?string $type = null;

    #[ORM\ManyToOne(inversedBy: 'computationResults')]
    #[ORM\JoinColumn(nullable: false)]
    private ?Computation $computation = null;

    public function getId(): ?int
    {
        return $this->id;
    }

    public function getTime(): ?int
    {
        return $this->time;
    }

    public function setTime(int $time): static
    {
        $this->time = $time;

        return $this;
    }

    public function getComputedValues(): array
    {
        return $this->computedValues;
    }

    public function setComputedValues(array $computedValues): static
    {
        $this->computedValues = $computedValues;

        return $this;
    }

    public function getType(): ?string
    {
        return $this->type;
    }

    public function setType(string $type): static
    {
        $this->type = $type;

        return $this;
    }

    public function getComputation(): ?Computation
    {
        return $this->computation;
    }

    public function setComputation(?Computation $computation): static
    {
        $this->computation = $computation;

        return $this;
    }
}
