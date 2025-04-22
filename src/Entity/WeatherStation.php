<?php

namespace App\Entity;

use App\Repository\WeatherStationRepository;
use Doctrine\ORM\Mapping as ORM;

#[ORM\Entity(repositoryClass: WeatherStationRepository::class)]
class WeatherStation
{
    #[ORM\Id]
    #[ORM\GeneratedValue]
    #[ORM\Column]
    private ?int $id = null;

    #[ORM\Column(length: 255)]
    private ?string $filename = null;

    #[ORM\Column]
    private ?\DateTimeImmutable $uploadedAt = null;

    #[ORM\ManyToOne(inversedBy: 'meteoFiles')]
    #[ORM\JoinColumn(nullable: false)]
    private ?User $uploadedBy = null;

    #[ORM\Column(length: 255)]
    private ?string $local_file_name = null;

    #[ORM\Column]
    private ?float $file_years = null;

    #[ORM\Column]
    private ?float $mechanical_annual_sodium = null;

    #[ORM\Column]
    private ?float $automatic_annual_sodium = null;

    public function getId(): ?int
    {
        return $this->id;
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

    public function getUploadedAt(): ?\DateTimeImmutable
    {
        return $this->uploadedAt;
    }

    public function setUploadedAt(\DateTimeImmutable $uploadedAt): static
    {
        $this->uploadedAt = $uploadedAt;

        return $this;
    }

    public function getUploadedBy(): ?User
    {
        return $this->uploadedBy;
    }

    public function setUploadedBy(?User $uploadedBy): static
    {
        $this->uploadedBy = $uploadedBy;

        return $this;
    }

    public function getLocalFileName(): ?string
    {
        return $this->local_file_name;
    }

    public function setLocalFileName(string $local_file_name): static
    {
        $this->local_file_name = $local_file_name;

        return $this;
    }

    public function getFileYears(): ?float
    {
        return $this->file_years;
    }

    public function setFileYears(float $file_years): static
    {
        $this->file_years = $file_years;

        return $this;
    }

    public function getMechanicalAnnualSodium(): ?float
    {
        return $this->mechanical_annual_sodium;
    }

    public function setMechanicalAnnualSodium(float $mechanical_annual_sodium): static
    {
        $this->mechanical_annual_sodium = $mechanical_annual_sodium;

        return $this;
    }

    public function getAutomaticAnnualSodium(): ?float
    {
        return $this->automatic_annual_sodium;
    }

    public function setAutomaticAnnualSodium(float $automatic_annual_sodium): static
    {
        $this->automatic_annual_sodium = $automatic_annual_sodium;

        return $this;
    }
}
