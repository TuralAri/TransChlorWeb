<?php

namespace App\Entity;

use App\Repository\ProbabilisticLawParamsRepository;
use Doctrine\ORM\Mapping as ORM;

#[ORM\Entity(repositoryClass: ProbabilisticLawParamsRepository::class)]
class ProbabilisticLawParams
{
    #[ORM\Id]
    #[ORM\GeneratedValue]
    #[ORM\Column]
    private ?int $id = null;

    #[ORM\Column]
    private ?float $meanValue = null;

    #[ORM\Column]
    private ?float $standardDeviation = null;

    #[ORM\Column]
    private ?float $lambda = null;

    #[ORM\Column]
    private ?float $ksi = null;

    #[ORM\Column]
    private ?float $pMinus = null;

    #[ORM\Column]
    private ?float $pPlus = null;

    #[ORM\Column]
    private ?float $x1 = null;

    #[ORM\Column]
    private ?float $x2 = null;

    public function getId(): ?int
    {
        return $this->id;
    }

    public function getMeanValue(): ?float
    {
        return $this->meanValue;
    }

    public function setMeanValue(float $meanValue): static
    {
        $this->meanValue = $meanValue;

        return $this;
    }

    public function getStandardDeviation(): ?float
    {
        return $this->standardDeviation;
    }

    public function setStandardDeviation(float $standardDeviation): static
    {
        $this->standardDeviation = $standardDeviation;

        return $this;
    }

    public function getLambda(): ?float
    {
        return $this->lambda;
    }

    public function setLambda(float $lambda): static
    {
        $this->lambda = $lambda;

        return $this;
    }

    public function getKsi(): ?float
    {
        return $this->ksi;
    }

    public function setKsi(float $ksi): static
    {
        $this->ksi = $ksi;

        return $this;
    }

    public function getPMinus(): ?float
    {
        return $this->pMinus;
    }

    public function setPMinus(float $pMinus): static
    {
        $this->pMinus = $pMinus;

        return $this;
    }

    public function getPPlus(): ?float
    {
        return $this->pPlus;
    }

    public function setPPlus(float $pPlus): static
    {
        $this->pPlus = $pPlus;

        return $this;
    }

    public function getX1(): ?float
    {
        return $this->x1;
    }

    public function setX1(float $x1): static
    {
        $this->x1 = $x1;

        return $this;
    }

    public function getX2(): ?float
    {
        return $this->x2;
    }

    public function setX2(float $x2): static
    {
        $this->x2 = $x2;

        return $this;
    }
}
