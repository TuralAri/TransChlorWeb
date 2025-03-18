<?php

namespace App\Repository;

use App\Entity\Meteo;
use Doctrine\Bundle\DoctrineBundle\Repository\ServiceEntityRepository;
use Doctrine\Persistence\ManagerRegistry;


/**
 * @extends ServiceEntityRepository<Meteo>
 */
class MeteoRepository extends ServiceEntityRepository {

    public function __construct(ManagerRegistry $registry)
    {
        parent::__construct($registry, Meteo::class);
    }

    public function findAll(): array
    {
        return $this->createQueryBuilder('m')
            ->orderBy('m.id', 'ASC')
            ->getQuery()
            ->getResult();
    }




}