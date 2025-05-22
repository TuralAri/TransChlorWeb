<?php

namespace App\Form;

use App\Entity\ProbabilisticLawParams;
use Symfony\Component\Form\AbstractType;
use Symfony\Component\Form\FormBuilderInterface;
use Symfony\Component\OptionsResolver\OptionsResolver;

class ProbabilisticLawFormType extends AbstractType
{
    public function buildForm(FormBuilderInterface $builder, array $options): void
    {
        $commonAttr = ['class' => 'ml-2 p-2 border rounded-lg text-left'];

        $builder
            ->add('meanValue', null, ['attr' => $commonAttr])
            ->add('standardDeviation', null, ['attr' => $commonAttr])
            ->add('lambda', null, ['attr' => $commonAttr])
            ->add('ksi', null, ['attr' => $commonAttr])
            ->add('pMinus', null, ['attr' => $commonAttr])
            ->add('pPlus', null, ['attr' => $commonAttr])
            ->add('x1', null, ['attr' => $commonAttr])
            ->add('x2', null, ['attr' => $commonAttr])
        ;
    }

    public function configureOptions(OptionsResolver $resolver): void
    {
        $resolver->setDefaults([
            'data_class' => ProbabilisticLawParams::class,
        ]);
    }
}
