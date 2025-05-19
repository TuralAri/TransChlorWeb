<?php

namespace App\Form;

use App\Entity\AggregateType;
use App\Entity\Material;
use Symfony\Bridge\Doctrine\Form\Type\EntityType;
use Symfony\Component\Form\AbstractType;
use Symfony\Component\Form\Extension\Core\Type\ChoiceType;
use Symfony\Component\Form\Extension\Core\Type\SubmitType;
use Symfony\Component\Form\FormBuilderInterface;
use Symfony\Component\OptionsResolver\OptionsResolver;

class MaterialFormType extends AbstractType
{
    public function buildForm(FormBuilderInterface $builder, array $options): void
    {
        $builder
            ->add('freshConcreteDensity')
            ->add('aggregateContent')
            ->add('cementContent')
            ->add('saturatedWaterContent')
            ->add('airContent')
            ->add('ec')
            ->add('concreteAge')
            ->add('hydrationRate')
            ->add('cementType', ChoiceType::class, [
                'choices' => [
                    'Type I' => '1',
                    'Type II' => '2',
                    'Type III' => '3',
                    'Type IV' => '4',
                ],
            ])
            ->add('heatCapacity')
            ->add('surfaceHeatTransfer')
            ->add('cementDensity')
            ->add('aggregateDensity')
            ->add('d100Percent')
            ->add('aoDiffusion')
            ->add('hc')
            ->add('ed')
            ->add('toDiffusion')
            ->add('surfaceTransferCoefficient')
            ->add('aoCapillarity')
            ->add('tc')
            ->add('dclTo')
            ->add('alphaDiffusion')
            ->add('toChlorideDiffusion')
            ->add('retardationCoefficient')
            ->add('limitWaterContent')
            ->add('adsorptionFa')
            ->add('alphaOh')
            ->add('eb')
            ->add('toAdsortion')
            ->add('aggregateType', EntityType::class, [
                'class' => AggregateType::class,
                'choice_label' => 'name',
                'choice_value' => 'aggregateDensity',
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
