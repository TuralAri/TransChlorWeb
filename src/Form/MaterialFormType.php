<?php

namespace App\Form;

use App\Entity\AggregateType;
use App\Entity\Material;
use App\Entity\Permeability;
use Doctrine\DBAL\Types\BooleanType;
use Symfony\Bridge\Doctrine\Form\Type\EntityType;
use Symfony\Component\Form\AbstractType;
use Symfony\Component\Form\Extension\Core\Type\ChoiceType;
use Symfony\Component\Form\Extension\Core\Type\NumberType;
use Symfony\Component\Form\Extension\Core\Type\SubmitType;
use Symfony\Component\Form\FormBuilderInterface;
use Symfony\Component\OptionsResolver\OptionsResolver;

class MaterialFormType extends AbstractType
{
    public function buildForm(FormBuilderInterface $builder, array $options): void
    {
        $builder
            ->add('freshConcreteDensity', NumberType::class, [
                'attr' => ['readonly' => true, 'value' => 2550],
                'label' => 'materialForm.labels.freshConcreteDensity',
            ])
            ->add('aggregateContent', NumberType::class, [
                'attr' => ['readonly' => true, 'value' => 2550],
                'label' => 'materialForm.labels.aggregateContent',
            ])
            ->add('cementContent', NumberType::class, [
                'attr' => ['value' => 0],
                'label' => 'materialForm.labels.cementContent',
            ])
            ->add('saturatedWaterContent', NumberType::class, [
                'attr' => ['value' => 0, 'readonly' => true],
                'label' => 'materialForm.labels.saturatedWaterContent',
            ])
            ->add('airContent', NumberType::class, [
                'attr' => ['value' => 0],
                'label' => 'materialForm.labels.airContent',
            ])
            ->add('ec', NumberType::class, [
                'attr' => ['value' => 0],
                'label' => 'materialForm.labels.ec',
            ])
            ->add('concreteAge', NumberType::class, [
                'attr' => ['value' => 0],
                'label' => 'materialForm.labels.concreteAge',
            ])
            ->add('hydrationRate', NumberType::class, [
                'attr' => ['value' => 0],
                'label' => 'materialForm.labels.hydrationRate',
            ])
            ->add('cementType', ChoiceType::class, [
                'choices' => [
                    'Type I' => '1',
                    'Type II' => '2',
                    'Type III' => '3',
                    'Type IV' => '4',
                ],
                'label' => 'materialForm.labels.cementType',
            ])
            ->add('permeability', EntityType::class, [
                'class' => Permeability::class,
                'choice_label' => 'name',
                'expanded' => true,
                'multiple' => false,
                'label' => 'materialForm.labels.permeability',
            ])
            ->add('heatCapacity', NumberType::class, [
                'attr' => ['value' => 0.7],
                'label' => 'materialForm.labels.heatCapacity',
            ])
            ->add('surfaceHeatTransfer', NumberType::class, [
                'attr' => ['value' => 1],
                'label' => 'materialForm.labels.surfaceHeatTransfer',
            ])
            ->add('cementDensity', NumberType::class, [
                'attr' => ['value' => 3150],
                'label' => 'materialForm.labels.cementDensity',
            ])
            ->add('aggregateDensity', NumberType::class, [
                'attr' => ['value' => 2550],
                'label' => 'materialForm.labels.aggregateDensity',
            ])
            ->add('d100Percent', NumberType::class, [
                'attr' => ['value' => 0],
                'label' => 'materialForm.labels.d100Percent',
            ])
            ->add('aoDiffusion', NumberType::class, [
                'attr' => ['value' => 0.05],
                'label' => 'materialForm.labels.aoDiffusion',
            ])
            ->add('hc', NumberType::class, [
                'attr' => ['value' => 0.75],
                'label' => 'materialForm.labels.hc',
            ])
            ->add('ed', NumberType::class, [
                'attr' => ['value' => 0],
                'label' => 'materialForm.labels.ed',
            ])
            ->add('toDiffusion', NumberType::class, [
                'attr' => ['value' => 293.16],
                'label' => 'materialForm.labels.toDiffusion',
            ])
            ->add('surfaceTransferCoefficient', NumberType::class, [
                'attr' => ['value' => 1],
                'label' => 'materialForm.labels.surfaceTransferCoefficient',
            ])
            ->add('aoCapillarity', NumberType::class, [
                'attr' => ['value' => 0.09],
                'label' => 'materialForm.labels.aoCapillarity',
            ])
            ->add('tc', NumberType::class, [
                'attr' => ['value' => 0.95],
                'label' => 'materialForm.labels.tc',
            ])
            ->add('dclTo', null, [
                'label' => 'materialForm.labels.dclTo',
            ])
            ->add('dclToValueBasedOnEc', null, [
                'label' => 'materialForm.labels.dclToValueBasedOnEc',
            ])
            ->add('alphaDiffusion', NumberType::class, [
                'attr' => ['value' => 0.026],
                'label' => 'materialForm.labels.alphaDiffusion',
            ])
            ->add('toChlorideDiffusion', NumberType::class, [
                'attr' => ['value' => 20],
                'label' => 'materialForm.labels.toChlorideDiffusion',
            ])
            ->add('retardationCoefficient', NumberType::class, [
                'attr' => ['value' => 0.7],
                'label' => 'materialForm.labels.retardationCoefficient',
            ])
            ->add('limitWaterContent', NumberType::class, [
                'attr' => ['value' => 0.8],
                'label' => 'materialForm.labels.limitWaterContent',
            ])
            ->add('adsorptionFa', NumberType::class, [
                'attr' => ['value' => 3.57],
                'label' => 'materialForm.labels.adsorptionFa',
            ])
            ->add('alphaOh', NumberType::class, [
                'attr' => ['value' => 0.56],
                'label' => 'materialForm.labels.alphaOh',
            ])
            ->add('eb', NumberType::class, [
                'attr' => ['value' => 0],
                'label' => 'materialForm.labels.eb',
            ])
            ->add('toAdsorption', NumberType::class, [
                'attr' => ['value' => 293.16],
                'label' => 'materialForm.labels.toAdsorption',
            ])
            ->add('aggregateType', EntityType::class, [
                'class' => AggregateType::class,
                'choice_label' => 'name',
                'choice_value' => 'id',
                'label' => 'materialForm.labels.aggregateType',
            ])
            ->add('submit', SubmitType::class, [
                'label' => 'materialForm.submit',
                'attr' => [
                    'class' => 'px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2'
                ]
            ]);
    }


    public function configureOptions(OptionsResolver $resolver): void
    {
        $resolver->setDefaults([
            'data_class' => Material::class,
        ]);
    }
}
