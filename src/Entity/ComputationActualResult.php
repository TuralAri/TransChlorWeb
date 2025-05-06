<?php

namespace App\Entity;

use App\Repository\ComputationActualResultRepository;
use Doctrine\DBAL\Types\Types;
use Doctrine\ORM\Mapping as ORM;

#[ORM\Entity(repositoryClass: ComputationActualResultRepository::class)]
class ComputationActualResult
{
    #[ORM\Id]
    #[ORM\GeneratedValue]
    #[ORM\Column]
    private ?int $id = null;

    #[ORM\ManyToOne(inversedBy: 'computationActualResults')]
    #[ORM\JoinColumn(nullable: false)]
    private ?Computation $computation = null;

    #[ORM\Column]
    private ?float $time = null;

    #[ORM\Column(type: Types::ARRAY)]
    private array $depths = [];

    #[ORM\Column(type: Types::ARRAY)]
    private array $computedValues = [];

    #[ORM\Column(length: 255)]
    private ?string $type = null;

    public function getId(): ?int
    {
        return $this->id;
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

    public function getTime(): ?float
    {
        return $this->time;
    }

    public function setTime(float $time): static
    {
        $this->time = $time;

        return $this;
    }

    public function getDepths(): array
    {
        return $this->depths;
    }

    public function setDepths(array $depths): static
    {
        $this->depths = $depths;

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
}
