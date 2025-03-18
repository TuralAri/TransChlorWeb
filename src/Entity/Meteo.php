<?php

namespace App\Entity;

use Doctrine\ORM\Mapping as ORM;

#[ORM\Entity]
class Meteo
{
    #[ORM\Id]
    #[ORM\GeneratedValue]
    #[ORM\Column(type: 'integer')]
    private int $id;

    #[ORM\Column(type: 'float')]
    private float $fileYears;

    #[ORM\Column(type: 'float')]
    private float $sodiumChlorideConcentration;

    #[ORM\Column(type: 'float')]
    private float $waterFilmThickness;

    #[ORM\Column(type: 'float')]
    private float $humidityThreshold;

    #[ORM\Column(type: 'float')]
    private float $mechanicalAnnualSodium;

    #[ORM\Column(type: 'float')]
    private float $mechanicalMeanSodium;

    #[ORM\Column(type: 'float')]
    private float $mechanicalInterventions;

    #[ORM\Column(type: 'float')]
    private float $mechanicalInterval;

    #[ORM\Column(type: 'float')]
    private float $mechanicalSodiumWater;

    #[ORM\Column(type: 'float', nullable: true)]
    private ?float $mechanicalThresholdTemperature = null;

    #[ORM\Column(type: 'float')]
    private float $automaticAnnualSodium;

    #[ORM\Column(type: 'float')]
    private float $automaticMeanSodium;

    #[ORM\Column(type: 'float')]
    private float $automaticSprays;

    #[ORM\Column(type: 'float')]
    private float $automaticSprayInterval;

    #[ORM\Column(type: 'float')]
    private float $automaticSodiumWater;

    #[ORM\Column(type: 'float', nullable: true)]
    private ?float $automaticThresholdTemperature = null;

    #[ORM\Column(type: 'float')]
    private float $extTemperaturePosition;

    #[ORM\Column(type: 'float')]
    private float $extTemperaturePosition2;

    #[ORM\Column(type: 'float')]
    private float $extTemperatureAttenuation;

    #[ORM\Column(type: 'float')]
    private float $extTemperatureAttenuation2;

    #[ORM\Column(type: 'float')]
    private float $extTemperatureDifference;

    #[ORM\Column(type: 'float')]
    private float $extHumidityPosition;

    #[ORM\Column(type: 'float')]
    private float $extHumidityPosition2;

    #[ORM\Column(type: 'float')]
    private float $extHumidityAttenuation;

    #[ORM\Column(type: 'float')]
    private float $extHumidityAttenuation2;

    #[ORM\Column(type: 'float')]
    private float $extHumidityDifference;

    #[ORM\Column(type: 'float')]
    private float $intTemperaturePosition;

    #[ORM\Column(type: 'float')]
    private float $intTemperaturePosition2;

    #[ORM\Column(type: 'float')]
    private float $intTemperatureAttenuation;

    #[ORM\Column(type: 'float')]
    private float $intTemperatureAttenuation2;

    #[ORM\Column(type: 'float')]
    private float $intTemperatureDifference;

    #[ORM\Column(type: 'float')]
    private float $intHumidityPosition;

    #[ORM\Column(type: 'float')]
    private float $intHumidityPosition2;

    #[ORM\Column(type: 'float')]
    private float $intHumidityAttenuation;

    #[ORM\Column(type: 'float')]
    private float $intHumidityAttenuation2;

    #[ORM\Column(type: 'float')]
    private float $intHumidityDifference;

    public function getId(): ?int
    {
        return $this->id;
    }

    public function getFileYears(): ?float
    {
        return $this->fileYears;
    }

    public function setFileYears(float $fileYears): self
    {
        $this->fileYears = $fileYears;
        return $this;
    }

    public function getSodiumChlorideConcentration(): ?float
    {
        return $this->sodiumChlorideConcentration;
    }

    public function setSodiumChlorideConcentration(float $sodiumChlorideConcentration): self
    {
        $this->sodiumChlorideConcentration = $sodiumChlorideConcentration;
        return $this;
    }

    public function getWaterFilmThickness(): ?float
    {
        return $this->waterFilmThickness;
    }

    public function setWaterFilmThickness(float $waterFilmThickness): self
    {
        $this->waterFilmThickness = $waterFilmThickness;
        return $this;
    }

    public function getHumidityThreshold(): ?float
    {
        return $this->humidityThreshold;
    }

    public function setHumidityThreshold(float $humidityThreshold): self
    {
        $this->humidityThreshold = $humidityThreshold;
        return $this;
    }

    public function getMechanicalAnnualSodium(): ?float
    {
        return $this->mechanicalAnnualSodium;
    }

    public function setMechanicalAnnualSodium(float $mechanicalAnnualSodium): self
    {
        $this->mechanicalAnnualSodium = $mechanicalAnnualSodium;
        return $this;
    }

    public function getMechanicalMeanSodium(): ?float
    {
        return $this->mechanicalMeanSodium;
    }

    public function setMechanicalMeanSodium(float $mechanicalMeanSodium): self
    {
        $this->mechanicalMeanSodium = $mechanicalMeanSodium;
        return $this;
    }

    public function getMechanicalInterventions(): ?float
    {
        return $this->mechanicalInterventions;
    }

    public function setMechanicalInterventions(float $mechanicalInterventions): self
    {
        $this->mechanicalInterventions = $mechanicalInterventions;
        return $this;
    }

    public function getMechanicalInterval(): ?float
    {
        return $this->mechanicalInterval;
    }

    public function setMechanicalInterval(float $mechanicalInterval): self
    {
        $this->mechanicalInterval = $mechanicalInterval;
        return $this;
    }

    public function getMechanicalSodiumWater(): ?float
    {
        return $this->mechanicalSodiumWater;
    }

    public function setMechanicalSodiumWater(float $mechanicalSodiumWater): self
    {
        $this->mechanicalSodiumWater = $mechanicalSodiumWater;
        return $this;
    }

    public function getMechanicalThresholdTemperature(): ?float
    {
        return $this->mechanicalThresholdTemperature;
    }

    public function setMechanicalThresholdTemperature(?float $mechanicalThresholdTemperature): self
    {
        $this->mechanicalThresholdTemperature = $mechanicalThresholdTemperature;
        return $this;
    }

    public function getAutomaticAnnualSodium(): ?float
    {
        return $this->automaticAnnualSodium;
    }

    public function setAutomaticAnnualSodium(float $automaticAnnualSodium): self
    {
        $this->automaticAnnualSodium = $automaticAnnualSodium;
        return $this;
    }

    public function getAutomaticMeanSodium(): ?float
    {
        return $this->automaticMeanSodium;
    }

    public function setAutomaticMeanSodium(float $automaticMeanSodium): self
    {
        $this->automaticMeanSodium = $automaticMeanSodium;
        return $this;
    }

    public function getAutomaticSprays(): ?float
    {
        return $this->automaticSprays;
    }

    public function setAutomaticSprays(float $automaticSprays): self
    {
        $this->automaticSprays = $automaticSprays;
        return $this;
    }

    public function getAutomaticSprayInterval(): ?float
    {
        return $this->automaticSprayInterval;
    }

    public function setAutomaticSprayInterval(float $automaticSprayInterval): self
    {
        $this->automaticSprayInterval = $automaticSprayInterval;
        return $this;
    }

    public function getAutomaticSodiumWater(): ?float
    {
        return $this->automaticSodiumWater;
    }

    public function setAutomaticSodiumWater(float $automaticSodiumWater): self
    {
        $this->automaticSodiumWater = $automaticSodiumWater;
        return $this;
    }

    public function getAutomaticThresholdTemperature(): ?float
    {
        return $this->automaticThresholdTemperature;
    }

    public function setAutomaticThresholdTemperature(?float $automaticThresholdTemperature): self
    {
        $this->automaticThresholdTemperature = $automaticThresholdTemperature;
        return $this;
    }

    public function getExtTemperaturePosition(): ?float
    {
        return $this->extTemperaturePosition;
    }

    public function setExtTemperaturePosition(float $extTemperaturePosition): self
    {
        $this->extTemperaturePosition = $extTemperaturePosition;
        return $this;
    }

    public function getExtTemperaturePosition2(): ?float
    {
        return $this->extTemperaturePosition2;
    }

    public function setExtTemperaturePosition2(float $extTemperaturePosition2): self
    {
        $this->extTemperaturePosition2 = $extTemperaturePosition2;
        return $this;
    }

    public function getExtTemperatureAttenuation(): ?float
    {
        return $this->extTemperatureAttenuation;
    }

    public function setExtTemperatureAttenuation(float $extTemperatureAttenuation): self
    {
        $this->extTemperatureAttenuation = $extTemperatureAttenuation;
        return $this;
    }

    public function getExtTemperatureAttenuation2(): ?float
    {
        return $this->extTemperatureAttenuation2;
    }

    public function setExtTemperatureAttenuation2(float $extTemperatureAttenuation2): self
    {
        $this->extTemperatureAttenuation2 = $extTemperatureAttenuation2;
        return $this;
    }

    public function getExtTemperatureDifference(): ?float
    {
        return $this->extTemperatureDifference;
    }

    public function setExtTemperatureDifference(float $extTemperatureDifference): self
    {
        $this->extTemperatureDifference = $extTemperatureDifference;
        return $this;
    }

    public function getExtHumidityPosition(): ?float
    {
        return $this->extHumidityPosition;
    }

    public function setExtHumidityPosition(float $extHumidityPosition): self
    {
        $this->extHumidityPosition = $extHumidityPosition;
        return $this;
    }

    public function getExtHumidityPosition2(): ?float
    {
        return $this->extHumidityPosition2;
    }

    public function setExtHumidityPosition2(float $extHumidityPosition2): self
    {
        $this->extHumidityPosition2 = $extHumidityPosition2;
        return $this;
    }

    public function getExtHumidityAttenuation(): ?float
    {
        return $this->extHumidityAttenuation;
    }

    public function setExtHumidityAttenuation(float $extHumidityAttenuation): self
    {
        $this->extHumidityAttenuation = $extHumidityAttenuation;
        return $this;
    }

    public function getExtHumidityAttenuation2(): ?float
    {
        return $this->extHumidityAttenuation2;
    }

    public function setExtHumidityAttenuation2(float $extHumidityAttenuation2): self
    {
        $this->extHumidityAttenuation2 = $extHumidityAttenuation2;
        return $this;
    }

    public function getExtHumidityDifference(): ?float
    {
        return $this->extHumidityDifference;
    }

    public function setExtHumidityDifference(float $extHumidityDifference): self
    {
        $this->extHumidityDifference = $extHumidityDifference;
        return $this;
    }

    public function getIntTemperaturePosition(): ?float
    {
        return $this->intTemperaturePosition;
    }

    public function setIntTemperaturePosition(float $intTemperaturePosition): self
    {
        $this->intTemperaturePosition = $intTemperaturePosition;
        return $this;
    }

    public function getIntTemperaturePosition2(): ?float
    {
        return $this->intTemperaturePosition2;
    }

    public function setIntTemperaturePosition2(float $intTemperaturePosition2): self
    {
        $this->intTemperaturePosition2 = $intTemperaturePosition2;
        return $this;
    }

    public function getIntTemperatureAttenuation(): ?float
    {
        return $this->intTemperatureAttenuation;
    }

    public function setIntTemperatureAttenuation(float $intTemperatureAttenuation): self
    {
        $this->intTemperatureAttenuation = $intTemperatureAttenuation;
        return $this;
    }

    public function getIntTemperatureAttenuation2(): ?float
    {
        return $this->intTemperatureAttenuation2;
    }

    public function setIntTemperatureAttenuation2(float $intTemperatureAttenuation2): self
    {
        $this->intTemperatureAttenuation2 = $intTemperatureAttenuation2;
        return $this;
    }

    public function getIntTemperatureDifference(): ?float
    {
        return $this->intTemperatureDifference;
    }

    public function setIntTemperatureDifference(float $intTemperatureDifference): self
    {
        $this->intTemperatureDifference = $intTemperatureDifference;
        return $this;
    }

    public function getIntHumidityPosition(): ?float
    {
        return $this->intHumidityPosition;
    }

    public function setIntHumidityPosition(float $intHumidityPosition): self
    {
        $this->intHumidityPosition = $intHumidityPosition;
        return $this;
    }

    public function getIntHumidityPosition2(): ?float
    {
        return $this->intHumidityPosition2;
    }

    public function setIntHumidityPosition2(float $intHumidityPosition2): self
    {
        $this->intHumidityPosition2 = $intHumidityPosition2;
        return $this;
    }

    public function getIntHumidityAttenuation(): ?float
    {
        return $this->intHumidityAttenuation;
    }

    public function setIntHumidityAttenuation(float $intHumidityAttenuation): self
    {
        $this->intHumidityAttenuation = $intHumidityAttenuation;
        return $this;
    }

    public function getIntHumidityAttenuation2(): ?float
    {
        return $this->intHumidityAttenuation2;
    }

    public function setIntHumidityAttenuation2(float $intHumidityAttenuation2): self
    {
        $this->intHumidityAttenuation2 = $intHumidityAttenuation2;
        return $this;
    }

    public function getIntHumidityDifference(): ?float
    {
        return $this->intHumidityDifference;
    }

    public function setIntHumidityDifference(float $intHumidityDifference): self
    {
        $this->intHumidityDifference = $intHumidityDifference;
        return $this;
    }
}