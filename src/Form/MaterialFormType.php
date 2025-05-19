<?php

namespace App\Form;

use App\Entity\AggregateType;
use App\Entity\Material;
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
            ->add('freshConcreteDensity', NumberType::class, ['attr' => ['readonly' => true, 'value' => 2550]])
            ->add('aggregateContent',NumberType::class, ['attr' => ['readonly' => true, 'value' => 2550]])
            ->add('cementContent', NumberType::class, ['attr' => ['value' => 0]])
            ->add('saturatedWaterContent', NumberType::class, ['attr' => ['value' => 0]])
            ->add('airContent', NumberType::class, ['attr' => ['value' => 0]])
            ->add('ec', NumberType::class, ['attr' => ['value' => 0]])
            ->add('concreteAge', NumberType::class, ['attr' => ['value' => 0]])
            ->add('hydrationRate', NumberType::class, ['attr' => ['value' => 0]])
            ->add('cementType', ChoiceType::class, [
                'choices' => [
                    'Type I' => '1',
                    'Type II' => '2',
                    'Type III' => '3',
                    'Type IV' => '4',
                ],
            ])
            ->add('heatCapacity', NumberType::class, ['attr' => ['value' => 0.7]])
            ->add('surfaceHeatTransfer', NumberType::class, ['attr' => ['value' => 1]])
            ->add('cementDensity', NumberType::class, ['attr' => ['value' => 3150]])
            ->add('aggregateDensity')
            ->add('d100Percent',NumberType::class, ['attr' => ['value' => 0]])
            ->add('aoDiffusion',NumberType::class, ['attr' => ['value' => 0.05]])
            ->add('hc', NumberType::class, ['attr' => ['value' => 0.75]])
            ->add('ed', NumberType::class, ['attr' => ['value' => 0]])
            ->add('toDiffusion', NumberType::class, ['attr' => ['value' => 293.16]])
            ->add('surfaceTransferCoefficient', NumberType::class, ['attr' => ['value' => 1]])
            ->add('aoCapillarity', NumberType::class, ['attr' => ['value' => 0.09]])
            ->add('tc', NumberType::class, ['attr' => ['value' => 0.95]])
            ->add('dclTo')
            ->add('alphaDiffusion', NumberType::class, ['attr' => ['value' => 0.026]])
            ->add('toChlorideDiffusion', NumberType::class, ['attr' => ['value' => 20]])
            ->add('retardationCoefficient', NumberType::class, ['attr' => ['value' => 0.7]])
            ->add('limitWaterContent', NumberType::class, ['attr' => ['value' => 0.8]])
            ->add('adsorptionFa', NumberType::class, ['attr' => ['value' => 3.57]])
            ->add('alphaOh', NumberType::class, ['attr' => ['value' => 0.56]])
            ->add('eb', NumberType::class, ['attr' => ['value' => 0]])
            ->add('toAdsortion', NumberType::class, ['attr' => ['value' => 293.16]])
            ->add('aggregateType', EntityType::class, [
                'class' => AggregateType::class,
                'choice_label' => 'name',
                'choice_value' => 'id',
            ])
            ->add('submit', SubmitType::class, [
                'label' => 'materialForm.submit',
                'attr' => [
                    'class' => 'px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2'
                ]
            ])
        ;
    }

    public function configureOptions(OptionsResolver $resolver): void
    {
        $resolver->setDefaults([
            'data_class' => Material::class,
        ]);
    }
}
