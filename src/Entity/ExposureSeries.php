<?php

namespace App\Entity;

use App\Repository\ExposureSeriesRepository;
use Doctrine\Common\Collections\ArrayCollection;
use Doctrine\Common\Collections\Collection;
use Doctrine\DBAL\Types\Types;
use Doctrine\ORM\Mapping as ORM;

#[ORM\Entity(repositoryClass: ExposureSeriesRepository::class)]
class ExposureSeries
{
    #[ORM\Id]
    #[ORM\GeneratedValue]
    #[ORM\Column]
    private ?int $id = null;

    #[ORM\ManyToOne(inversedBy: 'exposureSeries')]
    private ?WeatherStation $weatherStation = null;

    #[ORM\Column(length: 255)]
    private ?string $label = null;

    public function getLabel(): ?string
    {
        return $this->label;
    }

    public function setLabel(?string $label): void
    {
        $this->label = $label;
    }

    #[ORM\Column]
    private ?\DateTimeImmutable $createdAt = null;

    #[ORM\Column(type: Types::TEXT, nullable: true)]
    private ?string $comment = null;

    #[ORM\Column]
    private ?float $fileYears = null;

    #[ORM\Column]
    private ?float $sodiumChlorideConcentration = null;

    #[ORM\Column]
    private ?float $waterFilmThickness = null;

    #[ORM\Column]
    private ?float $humidityThreshold = null;

    #[ORM\Column]
    private ?float $mechanicalAnnualSodium = null;

    #[ORM\Column]
    private ?float $mechanicalMeanSodium = null;

    #[ORM\Column]
    private ?float $mechanicalInterventions = null;

    #[ORM\Column]
    private ?float $mechanicalInterval = null;

    #[ORM\Column]
    private ?float $mechanicalSodiumWater = null;

    #[ORM\Column]
    private ?float $mechanicalThresholdTemperature = null;

    #[ORM\Column]
    private ?float $automaticAnnualSodium = null;

    #[ORM\Column]
    private ?float $automaticMeanSodium = null;

    #[ORM\Column]
    private ?float $automaticSprays = null;

    #[ORM\Column]
    private ?float $automaticSprayInterval = null;

    #[ORM\Column]
    private ?float $automaticSodiumWater = null;

    #[ORM\Column]
    private ?float $automaticThresholdTemperature = null;

    #[ORM\Column]
    private ?float $extTemperaturePosition = null;

    #[ORM\Column]
    private ?float $extTemperaturePosition2 = null;

    #[ORM\Column]
    private ?float $extTemperatureAttenuation = null;

    #[ORM\Column]
    private ?float $extTemperatureAttenuation2 = null;

    #[ORM\Column]
    private ?float $extTemperatureDifference = null;

    #[ORM\Column]
    private ?float $extHumidityPosition = null;

    #[ORM\Column]
    private ?float $extHumidityPosition2 = null;

    #[ORM\Column]
    private ?float $extHumidityAttenuation = null;

    #[ORM\Column]
    private ?float $extHumidityAttenuation2 = null;

    #[ORM\Column]
    private ?float $extHumidityDifference = null;

    #[ORM\Column]
    private ?float $intTemperaturePosition = null;

    #[ORM\Column]
    private ?float $intTemperaturePosition2 = null;

    #[ORM\Column]
    private ?float $intTemperatureAttenuation = null;

    #[ORM\Column]
    private ?float $intTemperatureAttenuation2 = null;

    #[ORM\Column]
    private ?float $intTemperatureDifference = null;

    #[ORM\Column]
    private ?float $intHumidityPosition = null;

    #[ORM\Column]
    private ?float $intHumidityPosition2 = null;

    #[ORM\Column]
    private ?float $intHumidityAttenuation = null;

    #[ORM\Column]
    private ?float $intHumidityAttenuation2 = null;

    #[ORM\Column]
    private ?float $intHumidityDifference = null;

    /**
     * @var Collection<int, Exposure>
     */
    #[ORM\OneToMany(targetEntity: Exposure::class, mappedBy: 'ExposureSerie')]
    private Collection $exposures;

    public function __construct()
    {
        $this->exposures = new ArrayCollection();
        $this->createdAt = new \DateTimeImmutable('now');
        $this->initValues();
    }

    public function getId(): ?int
    {
        return $this->id;
    }

    public function getWeatherStation(): ?WeatherStation
    {
        return $this->weatherStation;
    }

    public function setWeatherStation(?WeatherStation $weatherStation): static
    {
        $this->weatherStation = $weatherStation;

        return $this;
    }

    public function getCreatedAt(): ?\DateTimeImmutable
    {
        return $this->createdAt;
    }

    public function setCreatedAt(\DateTimeImmutable $createdAt): static
    {
        $this->createdAt = $createdAt;

        return $this;
    }

    public function getComment(): ?string
    {
        return $this->comment;
    }

    public function setComment(?string $comment): static
    {
        $this->comment = $comment;

        return $this;
    }

    public function getFileYears(): ?float
    {
        return $this->fileYears;
    }

    public function setFileYears(float $fileYears): static
    {
        $this->fileYears = $fileYears;

        return $this;
    }

    public function getSodiumChlorideConcentration(): ?float
    {
        return $this->sodiumChlorideConcentration;
    }

    public function setSodiumChlorideConcentration(float $sodiumChlorideConcentration): static
    {
        $this->sodiumChlorideConcentration = $sodiumChlorideConcentration;

        return $this;
    }

    public function getWaterFilmThickness(): ?float
    {
        return $this->waterFilmThickness;
    }

    public function setWaterFilmThickness(float $waterFilmThickness): static
    {
        $this->waterFilmThickness = $waterFilmThickness;

        return $this;
    }

    public function getHumidityThreshold(): ?float
    {
        return $this->humidityThreshold;
    }

    public function setHumidityThreshold(float $humidityThreshold): static
    {
        $this->humidityThreshold = $humidityThreshold;

        return $this;
    }

    public function getMechanicalAnnualSodium(): ?float
    {
        return $this->mechanicalAnnualSodium;
    }

    public function setMechanicalAnnualSodium(float $mechanicalAnnualSodium): static
    {
        $this->mechanicalAnnualSodium = $mechanicalAnnualSodium;

        return $this;
    }

    public function getMechanicalMeanSodium(): ?float
    {
        return $this->mechanicalMeanSodium;
    }

    public function setMechanicalMeanSodium(float $mechanicalMeanSodium): static
    {
        $this->mechanicalMeanSodium = $mechanicalMeanSodium;

        return $this;
    }

    public function getMechanicalInterventions(): ?float
    {
        return $this->mechanicalInterventions;
    }

    public function setMechanicalInterventions(float $mechanicalInterventions): static
    {
        $this->mechanicalInterventions = $mechanicalInterventions;

        return $this;
    }

    public function getMechanicalInterval(): ?float
    {
        return $this->mechanicalInterval;
    }

    public function setMechanicalInterval(float $mechanicalInterval): static
    {
        $this->mechanicalInterval = $mechanicalInterval;

        return $this;
    }

    public function getMechanicalSodiumWater(): ?float
    {
        return $this->mechanicalSodiumWater;
    }

    public function setMechanicalSodiumWater(float $mechanicalSodiumWater): static
    {
        $this->mechanicalSodiumWater = $mechanicalSodiumWater;

        return $this;
    }

    public function getMechanicalThresholdTemperature(): ?float
    {
        return $this->mechanicalThresholdTemperature;
    }

    public function setMechanicalThresholdTemperature(float $mechanicalThresholdTemperature): static
    {
        $this->mechanicalThresholdTemperature = $mechanicalThresholdTemperature;

        return $this;
    }

    public function getAutomaticAnnualSodium(): ?float
    {
        return $this->automaticAnnualSodium;
    }

    public function setAutomaticAnnualSodium(float $automaticAnnualSodium): static
    {
        $this->automaticAnnualSodium = $automaticAnnualSodium;

        return $this;
    }

    public function getAutomaticMeanSodium(): ?float
    {
        return $this->automaticMeanSodium;
    }

    public function setAutomaticMeanSodium(float $automaticMeanSodium): static
    {
        $this->automaticMeanSodium = $automaticMeanSodium;

        return $this;
    }

    public function getAutomaticSprays(): ?float
    {
        return $this->automaticSprays;
    }

    public function setAutomaticSprays(float $automaticSprays): static
    {
        $this->automaticSprays = $automaticSprays;

        return $this;
    }

    public function getAutomaticSprayInterval(): ?float
    {
        return $this->automaticSprayInterval;
    }

    public function setAutomaticSprayInterval(float $automaticSprayInterval): static
    {
        $this->automaticSprayInterval = $automaticSprayInterval;

        return $this;
    }

    public function getAutomaticSodiumWater(): ?float
    {
        return $this->automaticSodiumWater;
    }

    public function setAutomaticSodiumWater(float $automaticSodiumWater): static
    {
        $this->automaticSodiumWater = $automaticSodiumWater;

        return $this;
    }

    public function getAutomaticThresholdTemperature(): ?float
    {
        return $this->automaticThresholdTemperature;
    }

    public function setAutomaticThresholdTemperature(float $automaticThresholdTemperature): static
    {
        $this->automaticThresholdTemperature = $automaticThresholdTemperature;

        return $this;
    }

    public function getExtTemperaturePosition(): ?float
    {
        return $this->extTemperaturePosition;
    }

    public function setExtTemperaturePosition(float $extTemperaturePosition): static
    {
        $this->extTemperaturePosition = $extTemperaturePosition;

        return $this;
    }

    public function getExtTemperaturePosition2(): ?float
    {
        return $this->extTemperaturePosition2;
    }

    public function setExtTemperaturePosition2(float $extTemperaturePosition2): static
    {
        $this->extTemperaturePosition2 = $extTemperaturePosition2;

        return $this;
    }

    public function getExtTemperatureAttenuation(): ?float
    {
        return $this->extTemperatureAttenuation;
    }

    public function setExtTemperatureAttenuation(float $extTemperatureAttenuation): static
    {
        $this->extTemperatureAttenuation = $extTemperatureAttenuation;

        return $this;
    }

    public function getExtTemperatureAttenuation2(): ?float
    {
        return $this->extTemperatureAttenuation2;
    }

    public function setExtTemperatureAttenuation2(float $extTemperatureAttenuation2): static
    {
        $this->extTemperatureAttenuation2 = $extTemperatureAttenuation2;

        return $this;
    }

    public function getExtTemperatureDifference(): ?float
    {
        return $this->extTemperatureDifference;
    }

    public function setExtTemperatureDifference(float $extTemperatureDifference): static
    {
        $this->extTemperatureDifference = $extTemperatureDifference;

        return $this;
    }

    public function getExtHumidityPosition(): ?float
    {
        return $this->extHumidityPosition;
    }

    public function setExtHumidityPosition(float $extHumidityPosition): static
    {
        $this->extHumidityPosition = $extHumidityPosition;

        return $this;
    }

    public function getExtHumidityPosition2(): ?float
    {
        return $this->extHumidityPosition2;
    }

    public function setExtHumidityPosition2(float $extHumidityPosition2): static
    {
        $this->extHumidityPosition2 = $extHumidityPosition2;

        return $this;
    }

    public function getExtHumidityAttenuation(): ?float
    {
        return $this->extHumidityAttenuation;
    }

    public function setExtHumidityAttenuation(float $extHumidityAttenuation): static
    {
        $this->extHumidityAttenuation = $extHumidityAttenuation;

        return $this;
    }

    public function getExtHumidityAttenuation2(): ?float
    {
        return $this->extHumidityAttenuation2;
    }

    public function setExtHumidityAttenuation2(float $extHumidityAttenuation2): static
    {
        $this->extHumidityAttenuation2 = $extHumidityAttenuation2;

        return $this;
    }

    public function getExtHumidityDifference(): ?float
    {
        return $this->extHumidityDifference;
    }

    public function setExtHumidityDifference(float $extHumidityDifference): static
    {
        $this->extHumidityDifference = $extHumidityDifference;

        return $this;
    }

    public function getIntTemperaturePosition(): ?float
    {
        return $this->intTemperaturePosition;
    }

    public function setIntTemperaturePosition(float $intTemperaturePosition): static
    {
        $this->intTemperaturePosition = $intTemperaturePosition;

        return $this;
    }

    public function getIntTemperaturePosition2(): ?float
    {
        return $this->intTemperaturePosition2;
    }

    public function setIntTemperaturePosition2(float $intTemperaturePosition2): static
    {
        $this->intTemperaturePosition2 = $intTemperaturePosition2;

        return $this;
    }

    public function getIntTemperatureAttenuation(): ?float
    {
        return $this->intTemperatureAttenuation;
    }

    public function setIntTemperatureAttenuation(float $intTemperatureAttenuation): static
    {
        $this->intTemperatureAttenuation = $intTemperatureAttenuation;

        return $this;
    }

    public function getIntTemperatureAttenuation2(): ?float
    {
        return $this->intTemperatureAttenuation2;
    }

    public function setIntTemperatureAttenuation2(float $intTemperatureAttenuation2): static
    {
        $this->intTemperatureAttenuation2 = $intTemperatureAttenuation2;

        return $this;
    }

    public function getIntTemperatureDifference(): ?float
    {
        return $this->intTemperatureDifference;
    }

    public function setIntTemperatureDifference(float $intTemperatureDifference): static
    {
        $this->intTemperatureDifference = $intTemperatureDifference;

        return $this;
    }

    public function getIntHumidityPosition(): ?float
    {
        return $this->intHumidityPosition;
    }

    public function setIntHumidityPosition(float $intHumidityPosition): static
    {
        $this->intHumidityPosition = $intHumidityPosition;

        return $this;
    }

    public function getIntHumidityPosition2(): ?float
    {
        return $this->intHumidityPosition2;
    }

    public function setIntHumidityPosition2(float $intHumidityPosition2): static
    {
        $this->intHumidityPosition2 = $intHumidityPosition2;

        return $this;
    }

    public function getIntHumidityAttenuation(): ?float
    {
        return $this->intHumidityAttenuation;
    }

    public function setIntHumidityAttenuation(float $intHumidityAttenuation): static
    {
        $this->intHumidityAttenuation = $intHumidityAttenuation;

        return $this;
    }

    public function getIntHumidityAttenuation2(): ?float
    {
        return $this->intHumidityAttenuation2;
    }

    public function setIntHumidityAttenuation2(float $intHumidityAttenuation2): static
    {
        $this->intHumidityAttenuation2 = $intHumidityAttenuation2;

        return $this;
    }

    public function getIntHumidityDifference(): ?float
    {
        return $this->intHumidityDifference;
    }

    public function setIntHumidityDifference(float $intHumidityDifference): static
    {
        $this->intHumidityDifference = $intHumidityDifference;

        return $this;
    }

    /**
     * @return Collection<int, Exposure>
     */
    public function getExposures(): Collection
    {
        return $this->exposures;
    }

    public function initValues()
    {
        //Valeurs partie haute formulaire
        $this->setSodiumChlorideConcentration(0.001);
        $this->setWaterFilmThickness(2.0);
        $this->setHumidityThreshold(95.0);
        //VALEURS EPANDAGE MECHANIQUE
        $this->setMechanicalMeanSodium(10.0);
        $this->setMechanicalInterval(8.0);
        $this->setMechanicalSodiumWater(36.0);
        //VALEURS EPANDAGE AUTOMATIQUE
        $this->setAutomaticMeanSodium(0.5);
        $this->setAutomaticSprayInterval(12.0);
        $this->setAutomaticSodiumWater(21.0);
        //Atténuation du signal (extérieur caisson)
            //Température
        $this->setExtTemperaturePosition(1.0);
        $this->setExtTemperaturePosition2(3.0);
        $this->setExtTemperatureAttenuation(1.0);
        $this->setExtTemperatureAttenuation2(2.0);
        $this->setExtTemperatureDifference(100.0);
            //Humidité relative
        $this->setExtHumidityPosition(2.0);
        $this->setExtHumidityPosition2(3.0);
        $this->setExtHumidityAttenuation(1.0);
        $this->setExtHumidityAttenuation2(4.0);
        $this->setExtHumidityDifference(100.0);
        //Atténuation du signal (intérieur caisson)
            //Température
        $this->setIntTemperaturePosition(1.0);
        $this->setIntTemperaturePosition2(3.0);
        $this->setIntTemperatureAttenuation(1.0);
        $this->setIntTemperatureAttenuation2(8.0);
        $this->setIntTemperatureDifference(100.0);
            //Humidité relative
        $this->setIntHumidityPosition(2.0);
        $this->setIntHumidityPosition2(3.0);
        $this->setIntHumidityAttenuation(1.0);
        $this->setIntHumidityAttenuation2(1.0);
        $this->setIntHumidityDifference(100.0);
    }

    public function addExposure(Exposure $exposure): static
    {
        if (!$this->exposures->contains($exposure)) {
            $this->exposures->add($exposure);
            $exposure->setExposureSerie($this);
        }

        return $this;
    }

    public function removeExposure(Exposure $exposure): static
    {
        if ($this->exposures->removeElement($exposure)) {
            // set the owning side to null (unless already changed)
            if ($exposure->getExposureSerie() === $this) {
                $exposure->setExposureSerie(null);
            }
        }

        return $this;
    }
}
