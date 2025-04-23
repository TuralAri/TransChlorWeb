<?php

namespace App\Entity;

use App\Repository\ExposureRepository;
use Doctrine\ORM\Mapping as ORM;

#[ORM\Entity(repositoryClass: ExposureRepository::class)]
class Exposure
{
    #[ORM\Id]
    #[ORM\GeneratedValue]
    #[ORM\Column]
    private ?int $id = null;

    #[ORM\ManyToOne(inversedBy: 'exposures')]
    private ?ExposureSeries $ExposureSerie = null;

    #[ORM\Column(length: 255)]
    private ?string $type = null;

    #[ORM\Column(length: 255)]
    private ?string $filename = null;

    #[ORM\Column(length: 255, nullable: true)]
    private ?string $localname = null;

    public function getId(): ?int
    {
        return $this->id;
    }

    public function getExposureSerie(): ?ExposureSeries
    {
        return $this->ExposureSerie;
    }

    public function setExposureSerie(?ExposureSeries $ExposureSerie): static
    {
        $this->ExposureSerie = $ExposureSerie;

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

    public function getFilename(): ?string
    {
        return $this->filename;
    }

    public function setFilename(string $filename): static
    {
        $this->filename = $filename;

        return $this;
    }

    public function getLocalname(): ?string
    {
        return $this->localname;
    }

    public function setLocalname(?string $localname): static
    {
        $this->localname = $localname;

        return $this;
    }
}
