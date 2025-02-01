<?php

namespace App\Entity;

use Doctrine\ORM\Mapping as ORM;

/**
 * @ORM\Entity()
 */
class Meteo
{
    /**
     * @ORM\Id
     * @ORM\GeneratedValue
     * @ORM\Column(type="integer")
     */
    private $id;

    /**
     * @ORM\Column(type="integer")
     */
    private $fileYears;

    /**
     * @ORM\Column(type="float")
     */
    private $sodiumChlorideConcentration;

    /**
     * @ORM\Column(type="float")
     */
    private $waterFilmThickness;

    /**
     * @ORM\Column(type="integer")
     */
    private $humidityThreshold;

    /**
     * @ORM\Column(type="float")
     */
    private $mechanicalAnnualSodium;

    /**
     * @ORM\Column(type="float")
     */
    private $mechanicalMeanSodium;

    /**
     * @ORM\Column(type="integer")
     */
    private $mechanicalInterventions;

    /**
     * @ORM\Column(type="float")
     */
    private $mechanicalInterval;

    /**
     * @ORM\Column(type="integer")
     */
    private $mechanicalSodiumWater;

    /**
     * @ORM\Column(type="float", nullable=true)
     */
    private $mechanicalThresholdTemperature;

    /**
     * @ORM\Column(type="float")
     */
    private $automaticAnnualSodium;

    /**
     * @ORM\Column(type="float")
     */
    private $automaticMeanSodium;

    /**
     * @ORM\Column(type="integer")
     */
    private $automaticSprays;

    /**
     * @ORM\Column(type="integer")
     */
    private $automaticSprayInterval;

    /**
     * @ORM\Column(type="integer")
     */
    private $automaticSodiumWater;

    /**
     * @ORM\Column(type="float", nullable=true)
     */
    private $automaticThresholdTemperature;

    /**
     * @ORM\Column(type="integer")
     */
    private $extTemperaturePosition;

    /**
     * @ORM\Column(type="integer")
     */
    private $extTemperaturePosition2;

    /**
     * @ORM\Column(type="integer")
     */
    private $extTemperatureAttenuation;

    /**
     * @ORM\Column(type="float")
     */
    private $extTemperatureAttenuation2;

    /**
     * @ORM\Column(type="float")
     */
    private $extTemperatureDifference;

    /**
     * @ORM\Column(type="integer")
     */
    private $extHumidityPosition;

    /**
     * @ORM\Column(type="integer")
     */
    private $extHumidityPosition2;

    /**
     * @ORM\Column(type="integer")
     */
    private $extHumidityAttenuation;

    /**
     * @ORM\Column(type="float")
     */
    private $extHumidityAttenuation2;

    /**
     * @ORM\Column(type="float")
     */
    private $extHumidityDifference;

    /**
     * @ORM\Column(type="integer")
     */
    private $intTemperaturePosition;

    /**
     * @ORM\Column(type="integer")
     */
    private $intTemperaturePosition2;

    /**
     * @ORM\Column(type="integer")
     */
    private $intTemperatureAttenuation;

    /**
     * @ORM\Column(type="float")
     */
    private $intTemperatureAttenuation2;

    /**
     * @ORM\Column(type="float")
     */
    private $intTemperatureDifference;

    /**
     * @ORM\Column(type="integer")
     */
    private $intHumidityPosition;

    /**
     * @ORM\Column(type="integer")
     */
    private $intHumidityPosition2;

    /**
     * @ORM\Column(type="integer")
     */
    private $intHumidityAttenuation;

    /**
     * @ORM\Column(type="float")
     */
    private $intHumidityAttenuation2;

    /**
     * @ORM\Column(type="float")
     */
    private $intHumidityDifference;

    public function getId(): ?int
    {
        return $this->id;
    }

    public function getFileYears(): ?int
    {
        return $this->fileYears;
    }

    public function setFileYears(int $fileYears): self
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

    public function getHumidityThreshold(): ?int
    {
        return $this->humidityThreshold;
    }

    public function setHumidityThreshold(int $humidityThreshold): self
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

    public function getMechanicalInterventions(): ?int
    {
        return $this->mechanicalInterventions;
    }

    public function setMechanicalInterventions(int $mechanicalInterventions): self
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

    public function getMechanicalSodiumWater(): ?int
    {
        return $this->mechanicalSodiumWater;
    }

    public function setMechanicalSodiumWater(int $mechanicalSodiumWater): self
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

    public function getAutomaticSprays(): ?int
    {
        return $this->automaticSprays;
    }

    public function setAutomaticSprays(int $automaticSprays): self
    {
        $this->automaticSprays = $automaticSprays;
        return $this;
    }

    public function getAutomaticSprayInterval(): ?int
    {
        return $this->automaticSprayInterval;
    }

    public function setAutomaticSprayInterval(int $automaticSprayInterval): self
    {
        $this->automaticSprayInterval = $automaticSprayInterval;
        return $this;
    }

    public function getAutomaticSodiumWater(): ?int
    {
        return $this->automaticSodiumWater;
    }

    public function setAutomaticSodiumWater(int $automaticSodiumWater): self
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

    public function getExtTemperaturePosition(): ?int
    {
        return $this->extTemperaturePosition;
    }

    public function setExtTemperaturePosition(int $extTemperaturePosition): self
    {
        $this->extTemperaturePosition = $extTemperaturePosition;
        return $this;
    }

    public function getExtTemperaturePosition2(): ?int
    {
        return $this->extTemperaturePosition2;
    }

    public function setExtTemperaturePosition2(int $extTemperaturePosition2): self
    {
        $this->extTemperaturePosition2 = $extTemperaturePosition2;
        return $this;
    }

    public function getExtTemperatureAttenuation(): ?int
    {
        return $this->extTemperatureAttenuation;
    }

    public function setExtTemperatureAttenuation(int $extTemperatureAttenuation): self
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

    public function getExtHumidityPosition(): ?int
    {
        return $this->extHumidityPosition;
    }

    public function setExtHumidityPosition(int $extHumidityPosition): self
    {
        $this->extHumidityPosition = $extHumidityPosition;
        return $this;
    }

    public function getExtHumidityPosition2(): ?int
    {
        return $this->extHumidityPosition2;
    }

    public function setExtHumidityPosition2(int $extHumidityPosition2): self
    {
        $this->extHumidityPosition2 = $extHumidityPosition2;
        return $this;
    }

    public function getExtHumidityAttenuation(): ?int
    {
        return $this->extHumidityAttenuation;
    }

    public function setExtHumidityAttenuation(int $extHumidityAttenuation): self
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

    public function getIntTemperaturePosition(): ?int
    {
        return $this->intTemperaturePosition;
    }

    public function setIntTemperaturePosition(int $intTemperaturePosition): self
    {
        $this->intTemperaturePosition = $intTemperaturePosition;
        return $this;
    }

    public function getIntTemperaturePosition2(): ?int
    {
        return $this->intTemperaturePosition2;
    }

    public function setIntTemperaturePosition2(int $intTemperaturePosition2): self
    {
        $this->intTemperaturePosition2 = $intTemperaturePosition2;
        return $this;
    }

    public function getIntTemperatureAttenuation(): ?int
    {
        return $this->intTemperatureAttenuation;
    }

    public function setIntTemperatureAttenuation(int $intTemperatureAttenuation): self
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

    public function getIntHumidityPosition(): ?int
    {
        return $this->intHumidityPosition;
    }

    public function setIntHumidityPosition(int $intHumidityPosition): self
    {
        $this->intHumidityPosition = $intHumidityPosition;
        return $this;
    }

    public function getIntHumidityPosition2(): ?int
    {
        return $this->intHumidityPosition2;
    }

    public function setIntHumidityPosition2(int $intHumidityPosition2): self
    {
        $this->intHumidityPosition2 = $intHumidityPosition2;
        return $this;
    }

    public function getIntHumidityAttenuation(): ?int
    {
        return $this->intHumidityAttenuation;
    }

    public function setIntHumidityAttenuation(int $intHumidityAttenuation): self
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

