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
            ->add('meanValue', null, [
                'attr' => $commonAttr,
                'required' => false,
                'empty_data' => null,
            ])
            ->add('standardDeviation', null, [
                'attr' => $commonAttr,
                'required' => false,
                'empty_data' => null,
            ])
            ->add('lambda', null, [
                'attr' => $commonAttr,
                'required' => false,
                'empty_data' => null,
            ])
            ->add('ksi', null, [
                'attr' => $commonAttr,
                'required' => false,
                'empty_data' => null,
            ])
            ->add('pMinus', null, [
                'attr' => $commonAttr,
                'required' => false,
                'empty_data' => null,
            ])
            ->add('pPlus', null, [
                'attr' => $commonAttr,
                'required' => false,
                'empty_data' => null,
            ])
            ->add('x1', null, [
                'attr' => $commonAttr,
                'required' => false,
                'empty_data' => null,
            ])
            ->add('x2', null, [
                'attr' => $commonAttr,
                'required' => false,
                'empty_data' => null,
            ])
        ;
    }

    public function configureOptions(OptionsResolver $resolver): void
    {
        $resolver->setDefaults([
            'data_class' => ProbabilisticLawParams::class,
        ]);
    }
}
