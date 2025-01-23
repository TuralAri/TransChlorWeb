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
     * @ORM\Column(type="string", length=255)
     */
    private $textBox1;

    /**
     * @ORM\Column(type="string", length=255)
     */
    private $textBox2;

    /**
     * @ORM\Column(type="integer")
     */
    private $numericUpDown1;

    /**
     * @ORM\Column(type="integer")
     */
    private $numericUpDown2;

    // Add additional properties as needed for the other fields

    public function getId(): ?int
    {
        return $this->id;
    }

    public function getTextBox1(): ?string
    {
        return $this->textBox1;
    }

    public function setTextBox1(string $textBox1): self
    {
        $this->textBox1 = $textBox1;

        return $this;
    }

    public function getTextBox2(): ?string
    {
        return $this->textBox2;
    }

    public function setTextBox2(string $textBox2): self
    {
        $this->textBox2 = $textBox2;

        return $this;
    }

    public function getNumericUpDown1(): ?int
    {
        return $this->numericUpDown1;
    }

    public function setNumericUpDown1(int $numericUpDown1): self
    {
        $this->numericUpDown1 = $numericUpDown1;

        return $this;
    }

    public function getNumericUpDown2(): ?int
    {
        return $this->numericUpDown2;
    }

    public function setNumericUpDown2(int $numericUpDown2): self
    {
        $this->numericUpDown2 = $numericUpDown2;

        return $this;
    }
}
