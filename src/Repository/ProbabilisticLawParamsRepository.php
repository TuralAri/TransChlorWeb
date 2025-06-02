<?php

namespace App\Repository;

use App\Entity\Input;
use App\Entity\Material;
use App\Entity\ProbabilisticLawParams;
use Doctrine\Bundle\DoctrineBundle\Repository\ServiceEntityRepository;
use Doctrine\ORM\Query\Parameter;
use Doctrine\Persistence\ManagerRegistry;
use Doctrine\Common\Collections\ArrayCollection;

/**
 * @extends ServiceEntityRepository<ProbabilisticLawParams>
 */
class ProbabilisticLawParamsRepository extends ServiceEntityRepository
{
    public function __construct(ManagerRegistry $registry)
    {
        parent::__construct($registry, ProbabilisticLawParams::class);
    }

    public function findOneByMaterialInputAndType(Material $material, Input $input, string $type): ?ProbabilisticLawParams
    {
        return $this->createQueryBuilder('p')
            ->andWhere('p.material = :material')
            ->andWhere('p.input = :input')
            ->andWhere('p.type = :type')
            ->setParameters(new ArrayCollection([
                new Parameter('material', $material),
                new Parameter('input', $input),
                new Parameter('type', $type),
            ]))
            ->getQuery()
            ->getOneOrNullResult();
    }

    //    /**
    //     * @return ProbabilisticLawParams[] Returns an array of ProbabilisticLawParams objects
    //     */
    //    public function findByExampleField($value): array
    //    {
    //        return $this->createQueryBuilder('p')
    //            ->andWhere('p.exampleField = :val')
    //            ->setParameter('val', $value)
    //            ->orderBy('p.id', 'ASC')
    //            ->setMaxResults(10)
    //            ->getQuery()
    //            ->getResult()
    //        ;
    //    }

    //    public function findOneBySomeField($value): ?ProbabilisticLawParams
    //    {
    //        return $this->createQueryBuilder('p')
    //            ->andWhere('p.exampleField = :val')
    //            ->setParameter('val', $value)
    //            ->getQuery()
    //            ->getOneOrNullResult()
    //        ;
    //    }
}
