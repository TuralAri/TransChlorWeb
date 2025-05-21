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
use Symfony\Component\Form\Extension\Core\Type\TextType;
use Symfony\Component\Form\FormBuilderInterface;
use Symfony\Component\OptionsResolver\OptionsResolver;

class MaterialFormType extends AbstractType
{
    public function buildForm(FormBuilderInterface $builder, array $options): void
    {
        $builder
            ->add('name', null, [
                'label' => 'materialForm.labels.name',
            ])
            ->add('comment', null, [
                'label' => 'materialForm.labels.comment',
            ])
            ->add('freshConcreteDensity', TextType::class, [
                'attr' => ['readonly' => true,],
                'label' => 'materialForm.labels.freshConcreteDensity',
            ])
            ->add('aggregateContent', TextType::class, [
                'attr' => ['readonly' => true,],
                'label' => 'materialForm.labels.aggregateContent',
            ])
            ->add('cementContent', TextType::class, [
                'label' => 'materialForm.labels.cementContent',
            ])
            ->add('saturatedWaterContent', TextType::class, [
                'attr' => ['readonly' => true],
                'label' => 'materialForm.labels.saturatedWaterContent',
            ])
            ->add('airContent', TextType::class, [
                'label' => 'materialForm.labels.airContent',
            ])
            ->add('ec', TextType::class, [
                'label' => 'materialForm.labels.ec',
            ])
            ->add('concreteAge', TextType::class, [
                'label' => 'materialForm.labels.concreteAge',
            ])
            ->add('hydrationRate', TextType::class, [
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
            ->add('heatCapacity', TextType::class, [
                'label' => 'materialForm.labels.heatCapacity',
            ])
            ->add('surfaceHeatTransfer', TextType::class, [
                'label' => 'materialForm.labels.surfaceHeatTransfer',
            ])
            ->add('cementDensity', TextType::class, [
                'label' => 'materialForm.labels.cementDensity',
            ])
            ->add('aggregateDensity', TextType::class, [
                'label' => 'materialForm.labels.aggregateDensity',
            ])
            ->add('d100Percent', TextType::class, [
                'label' => 'materialForm.labels.d100Percent',
            ])
            ->add('aoDiffusion', TextType::class, [
                'label' => 'materialForm.labels.aoDiffusion',
            ])
            ->add('hc', TextType::class, [
                'label' => 'materialForm.labels.hc',
            ])
            ->add('ed', TextType::class, [
                'label' => 'materialForm.labels.ed',
            ])
            ->add('toDiffusion', TextType::class, [
                'label' => 'materialForm.labels.toDiffusion',
            ])
            ->add('surfaceTransferCoefficient', TextType::class, [
                'label' => 'materialForm.labels.surfaceTransferCoefficient',
            ])
            ->add('aoCapillarity', TextType::class, [
                'label' => 'materialForm.labels.aoCapillarity',
            ])
            ->add('tc', TextType::class, [
                'label' => 'materialForm.labels.tc',
            ])
            ->add('dclTo', TextType::class, [
                'label' => 'materialForm.labels.dclTo',
            ])
            ->add('dclToValueBasedOnEc', null, [
                'label' => 'materialForm.labels.dclToValueBasedOnEc',
            ])
            ->add('alphaDiffusion', TextType::class, [
                'label' => 'materialForm.labels.alphaDiffusion',
            ])
            ->add('toChlorideDiffusion', TextType::class, [
                'label' => 'materialForm.labels.toChlorideDiffusion',
            ])
            ->add('retardationCoefficient', TextType::class, [
                'label' => 'materialForm.labels.retardationCoefficient',
            ])
            ->add('limitWaterContent', TextType::class, [
                'label' => 'materialForm.labels.limitWaterContent',
            ])
            ->add('adsorptionFa', TextType::class, [
                'label' => 'materialForm.labels.adsorptionFa',
            ])
            ->add('alphaOh', TextType::class, [
                'label' => 'materialForm.labels.alphaOh',
            ])
            ->add('eb', TextType::class, [
                'label' => 'materialForm.labels.eb',
            ])
            ->add('toAdsorption', TextType::class, [
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
