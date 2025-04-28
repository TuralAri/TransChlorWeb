<?php

namespace App\Entity;

use App\Repository\WeatherStationRepository;
use Doctrine\Common\Collections\ArrayCollection;
use Doctrine\Common\Collections\Collection;
use Doctrine\DBAL\Types\Types;
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

    /**
     * @var Collection<int, ExposureSeries>
     */
    #[ORM\OneToMany(targetEntity: ExposureSeries::class, mappedBy: 'weatherStation')]
    private Collection $exposureSeries;

    #[ORM\Column(nullable: true)]
    private ?int $stationNumber = null;

    #[ORM\Column(type: Types::DATETIME_MUTABLE, nullable: true)]
    private ?\DateTimeInterface $startDate = null;

    #[ORM\Column(type: Types::DATETIME_MUTABLE, nullable: true)]
    private ?\DateTimeInterface $endDate = null;

    public function __construct()
    {
        $this->exposureSeries = new ArrayCollection();
        $this->uploadedAt = new \DateTimeImmutable('now');
    }

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

    public function setMeteoFileVars($meteoFileVars)
    {
        $this->setStationNumber($meteoFileVars['stationNumber']);
        $this->setLocalFileName($meteoFileVars['stationName']);
        $this->setStartDate($meteoFileVars['startDate']);
        $this->setEndDate($meteoFileVars['endDate']);
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

    /**
     * @return Collection<int, ExposureSeries>
     */
    public function getExposureSeries(): Collection
    {
        return $this->exposureSeries;
    }

    public function addExposureSeries(ExposureSeries $exposureSeries): static
    {
        if (!$this->exposureSeries->contains($exposureSeries)) {
            $this->exposureSeries->add($exposureSeries);
            $exposureSeries->setWeatherStation($this);
        }

        return $this;
    }

    public function removeExposureSeries(ExposureSeries $exposureSeries): static
    {
        if ($this->exposureSeries->removeElement($exposureSeries)) {
            // set the owning side to null (unless already changed)
            if ($exposureSeries->getWeatherStation() === $this) {
                $exposureSeries->setWeatherStation(null);
            }
        }

        return $this;
    }

    public function getStationNumber(): ?int
    {
        return $this->stationNumber;
    }

    public function setStationNumber(?int $stationNumber): static
    {
        $this->stationNumber = $stationNumber;

        return $this;
    }

    public function getStartDate(): ?\DateTimeInterface
    {
        return $this->startDate;
    }

    public function setStartDate(?\DateTimeInterface $startDate): static
    {
        $this->startDate = $startDate;

        return $this;
    }

    public function getEndDate(): ?\DateTimeInterface
    {
        return $this->endDate;
    }

    public function setEndDate(?\DateTimeInterface $endDate): static
    {
        $this->endDate = $endDate;

        return $this;
    }
}
