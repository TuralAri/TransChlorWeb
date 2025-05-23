<?php

namespace App\Entity;

use App\Repository\UserRepository;
use Doctrine\Common\Collections\ArrayCollection;
use Doctrine\Common\Collections\Collection;
use Doctrine\ORM\Mapping as ORM;
use Symfony\Bridge\Doctrine\Validator\Constraints\UniqueEntity;
use Symfony\Component\Security\Core\User\PasswordAuthenticatedUserInterface;
use Symfony\Component\Security\Core\User\UserInterface;

#[ORM\Entity(repositoryClass: UserRepository::class)]
#[ORM\Table(name: 'user')]
#[ORM\UniqueConstraint(name: 'UNIQ_IDENTIFIER_USERNAME', fields: ['username'])]
#[UniqueEntity(fields: ['username'], message: 'There is already an account with this username')]
class User implements UserInterface, PasswordAuthenticatedUserInterface
{
    #[ORM\Id]
    #[ORM\GeneratedValue]
    #[ORM\Column]
    private ?int $id = null;

    #[ORM\Column(length: 180)]
    private ?string $username = null;

    /**
     * @var list<string> The user roles
     */
    #[ORM\Column]
    private array $roles = [];

    /**
     * @var string The hashed password
     */
    #[ORM\Column]
    private ?string $password = null;

    /**
     * @var Collection<int, WeatherStation>
     */
    #[ORM\OneToMany(targetEntity: WeatherStation::class, mappedBy: 'uploadedBy')]
    private Collection $meteoFiles;

    /**
     * @var Collection<int, Material>
     */
    #[ORM\OneToMany(targetEntity: Material::class, mappedBy: 'user', orphanRemoval: true)]
    private Collection $materials;

    /**
     * @var Collection<int, Input>
     */
    #[ORM\OneToMany(targetEntity: Input::class, mappedBy: 'user', orphanRemoval: true)]
    private Collection $inputs;

    public function __construct()
    {
        $this->meteoFiles = new ArrayCollection();
        $this->materials = new ArrayCollection();
        $this->inputs = new ArrayCollection();
    }

    public function getId(): ?int
    {
        return $this->id;
    }

    public function getUsername(): ?string
    {
        return $this->username;
    }

    public function setUsername(string $username): static
    {
        $this->username = $username;

        return $this;
    }

    /**
     * A visual identifier that represents this user.
     *
     * @see UserInterface
     */
    public function getUserIdentifier(): string
    {
        return (string)$this->username;
    }

    /**
     * @return list<string>
     * @see UserInterface
     */
    public function getRoles(): array
    {
        $roles = $this->roles;
        // guarantee every user at least has ROLE_USER
        $roles[] = 'ROLE_USER';

        return array_unique($roles);
    }

    /**
     * @param list<string> $roles
     */
    public function setRoles(array $roles): static
    {
        $this->roles = $roles;

        return $this;
    }

    /**
     * @see PasswordAuthenticatedUserInterface
     */
    public function getPassword(): ?string
    {
        return $this->password;
    }

    public function setPassword(string $password): static
    {
        $this->password = $password;

        return $this;
    }

    /**
     * @see UserInterface
     */
    public function eraseCredentials(): void
    {
        // If you store any temporary, sensitive data on the user, clear it here
        // $this->plainPassword = null;
    }

    /**
     * @return Collection<int, WeatherStation>
     */
    public function getMeteoFiles(): Collection
    {
        return $this->meteoFiles;
    }

    public function addMeteoFile(WeatherStation $meteoFile): static
    {
        if (!$this->meteoFiles->contains($meteoFile)) {
            $this->meteoFiles->add($meteoFile);
            $meteoFile->setUploadedBy($this);
        }

        return $this;
    }

    public function removeMeteoFile(WeatherStation $meteoFile): static
    {
        if ($this->meteoFiles->removeElement($meteoFile)) {
            // set the owning side to null (unless already changed)
            if ($meteoFile->getUploadedBy() === $this) {
                $meteoFile->setUploadedBy(null);
            }
        }

        return $this;
    }

    /**
     * @return Collection<int, Material>
     */
    public function getMaterials(): Collection
    {
        return $this->materials;
    }

    public function addMaterial(Material $material): static
    {
        if (!$this->materials->contains($material)) {
            $this->materials->add($material);
            $material->setUser($this);
        }

        return $this;
    }

    public function removeMaterial(Material $material): static
    {
        if ($this->materials->removeElement($material)) {
            // set the owning side to null (unless already changed)
            if ($material->getUser() === $this) {
                $material->setUser(null);
            }
        }

        return $this;
    }

    /**
     * @return Collection<int, Input>
     */
    public function getInputs(): Collection
    {
        return $this->inputs;
    }

    public function addInput(Input $input): static
    {
        if (!$this->inputs->contains($input)) {
            $this->inputs->add($input);
            $input->setUser($this);
        }

        return $this;
    }

    public function removeInput(Input $input): static
    {
        if ($this->inputs->removeElement($input)) {
            // set the owning side to null (unless already changed)
            if ($input->getUser() === $this) {
                $input->setUser(null);
            }
        }

        return $this;
    }
}