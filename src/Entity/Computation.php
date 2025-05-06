<?php

namespace App\Entity;

use App\Repository\ComputationRepository;
use Doctrine\Common\Collections\ArrayCollection;
use Doctrine\Common\Collections\Collection;
use Doctrine\DBAL\Types\Types;
use Doctrine\ORM\Mapping as ORM;

#[ORM\Entity(repositoryClass: ComputationRepository::class)]
class Computation
{
    #[ORM\Id]
    #[ORM\GeneratedValue]
    #[ORM\Column]
    private ?int $id = null;

    #[ORM\Column(length: 255)]
    private ?string $status = null;

    #[ORM\Column(type: Types::DATETIME_MUTABLE)]
    private ?\DateTimeInterface $startDate = null;

    #[ORM\Column(type: Types::DATETIME_MUTABLE)]
    private ?\DateTimeInterface $endDate = null;

    /**
     * @var Collection<int, ComputationResult>
     */
    #[ORM\OneToMany(targetEntity: ComputationResult::class, mappedBy: 'computation', orphanRemoval: true)]
    private Collection $computationResults;

    /**
     * @var Collection<int, ComputationActualResult>
     */
    #[ORM\OneToMany(targetEntity: ComputationActualResult::class, mappedBy: 'computation', orphanRemoval: true)]
    private Collection $computationActualResults;

    public function __construct()
    {
        $this->computationResults = new ArrayCollection();
        $this->computationActualResults = new ArrayCollection();
    }

    public function getId(): ?int
    {
        return $this->id;
    }

    public function getStatus(): ?string
    {
        return $this->status;
    }

    public function setStatus(string $status): static
    {
        $this->status = $status;

        return $this;
    }

    public function getStartDate(): ?\DateTimeInterface
    {
        return $this->startDate;
    }

    public function setStartDate(\DateTimeInterface $startDate): static
    {
        $this->startDate = $startDate;

        return $this;
    }

    public function getEndDate(): ?\DateTimeInterface
    {
        return $this->endDate;
    }

    public function setEndDate(\DateTimeInterface $endDate): static
    {
        $this->endDate = $endDate;

        return $this;
    }

    /**
     * @return Collection<int, ComputationResult>
     */
    public function getComputationResults(): Collection
    {
        return $this->computationResults;
    }

    public function addComputationResult(ComputationResult $computationResult): static
    {
        if (!$this->computationResults->contains($computationResult)) {
            $this->computationResults->add($computationResult);
            $computationResult->setComputation($this);
        }

        return $this;
    }

    public function removeComputationResult(ComputationResult $computationResult): static
    {
        if ($this->computationResults->removeElement($computationResult)) {
            // set the owning side to null (unless already changed)
            if ($computationResult->getComputation() === $this) {
                $computationResult->setComputation(null);
            }
        }

        return $this;
    }

    /**
     * @return Collection<int, ComputationActualResult>
     */
    public function getComputationActualResults(): Collection
    {
        return $this->computationActualResults;
    }

    public function addComputationActualResult(ComputationActualResult $computationActualResult): static
    {
        if (!$this->computationActualResults->contains($computationActualResult)) {
            $this->computationActualResults->add($computationActualResult);
            $computationActualResult->setComputation($this);
        }

        return $this;
    }

    public function removeComputationActualResult(ComputationActualResult $computationActualResult): static
    {
        if ($this->computationActualResults->removeElement($computationActualResult)) {
            // set the owning side to null (unless already changed)
            if ($computationActualResult->getComputation() === $this) {
                $computationActualResult->setComputation(null);
            }
        }

        return $this;
    }
}
