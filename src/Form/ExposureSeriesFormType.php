<?php

namespace App\Form;

use App\Entity\ExposureSeries;
use App\Entity\WeatherStation;
use Symfony\Bridge\Doctrine\Form\Type\EntityType;
use Symfony\Component\Form\AbstractType;
use Symfony\Component\Form\Extension\Core\Type\NumberType;
use Symfony\Component\Form\Extension\Core\Type\SubmitType;
use Symfony\Component\Form\Extension\Core\Type\TextareaType;
use Symfony\Component\Form\Extension\Core\Type\TextType;
use Symfony\Component\Form\FormBuilderInterface;
use Symfony\Component\OptionsResolver\OptionsResolver;

class ExposureSeriesFormType extends AbstractType
{
    public function buildForm(FormBuilderInterface $builder, array $options): void
    {
        $builder
            ->add('label', TextType::class, ['label' => 'exposureSeriesForm.seriesLabel'])
            ->add('comment', TextAreaType::class, ['label' => 'exposureSeriesForm.comment', 'required' => false])
            ->add('fileYears', NumberType::class, ['label' => 'exposureSeriesForm.yearNumber', 'attr' => ['readonly' => true]])
            ->add('sodiumChlorideConcentration', NumberType::class, ['label' => 'exposureSeriesForm.sodiumChlorideConcentration'])
            ->add('waterFilmThickness', NumberType::class, ['label' => 'exposureSeriesForm.waterFilmThickness'])
            ->add('humidityThreshold', NumberType::class, ['label' => 'exposureSeriesForm.humidityThreshold'])

            ->add('mechanicalAnnualSodium', NumberType::class, ['label' => 'exposureSeriesForm.mechanicalAnnualSodium'])
            ->add('mechanicalMeanSodium', NumberType::class, ['label' => 'exposureSeriesForm.mechanicalMeanSodium'])
            ->add('mechanicalInterventions', NumberType::class, ['label' => 'exposureSeriesForm.mechanicalInterventions', 'required' => false, 'attr' => ['readonly' => true]])
            ->add('mechanicalInterval', NumberType::class, ['label' => 'exposureSeriesForm.mechanicalInterval'])
            ->add('mechanicalSodiumWater', NumberType::class, ['label' => 'exposureSeriesForm.mechanicalSodiumWater'])
            ->add('mechanicalThresholdTemperature', NumberType::class, ['label' => 'exposureSeriesForm.mechanicalThresholdTemperature', 'required' => false, 'attr' => ['readonly' => true]])

            ->add('automaticAnnualSodium', NumberType::class, ['label' => 'exposureSeriesForm.automaticAnnualSodium'])
            ->add('automaticMeanSodium', NumberType::class, ['label' => 'exposureSeriesForm.automaticMeanSodium'])
            ->add('automaticSprays', NumberType::class, ['label' => 'exposureSeriesForm.automaticSprays', 'required' => false, 'attr' => ['readonly' => true]])
            ->add('automaticSprayInterval', NumberType::class, ['label' => 'exposureSeriesForm.automaticSprayInterval'])
            ->add('automaticSodiumWater', NumberType::class, ['label' => 'exposureSeriesForm.automaticSodiumWater'])
            ->add('automaticThresholdTemperature', NumberType::class, ['label' => 'exposureSeriesForm.automaticThresholdTemperature', 'required' => false, 'attr' => ['readonly' => true]])

            ->add('extTemperaturePosition', NumberType::class, ['label' => 'exposureSeriesForm.TemperaturePosition'])
            ->add('extTemperaturePosition2', NumberType::class, ['label' => 'exposureSeriesForm.TemperaturePosition2'])
            ->add('extTemperatureAttenuation', NumberType::class, ['label' => 'exposureSeriesForm.TemperatureAttenuation'])
            ->add('extTemperatureAttenuation2', NumberType::class, ['label' => 'exposureSeriesForm.TemperatureAttenuation2'])
            ->add('extTemperatureDifference', NumberType::class, ['label' => 'exposureSeriesForm.TemperatureDifference'])

            ->add('extHumidityPosition', NumberType::class, ['label' => 'exposureSeriesForm.HumidityPosition'])
            ->add('extHumidityPosition2', NumberType::class, ['label' => 'exposureSeriesForm.HumidityPosition2'])
            ->add('extHumidityAttenuation', NumberType::class, ['label' => 'exposureSeriesForm.HumidityAttenuation'])
            ->add('extHumidityAttenuation2', NumberType::class, ['label' => 'exposureSeriesForm.HumidityAttenuation2'])
            ->add('extHumidityDifference', NumberType::class, ['label' => 'exposureSeriesForm.HumidityDifference'])

            ->add('intTemperaturePosition', NumberType::class, ['label' => 'exposureSeriesForm.TemperaturePosition'])
            ->add('intTemperaturePosition2', NumberType::class, ['label' => 'exposureSeriesForm.TemperaturePosition2'])
            ->add('intTemperatureAttenuation', NumberType::class, ['label' => 'exposureSeriesForm.TemperatureAttenuation'])
            ->add('intTemperatureAttenuation2', NumberType::class, ['label' => 'exposureSeriesForm.TemperatureAttenuation2'])
            ->add('intTemperatureDifference', NumberType::class, ['label' => 'exposureSeriesForm.TemperatureDifference'])

            ->add('intHumidityPosition', NumberType::class, ['label' => 'exposureSeriesForm.HumidityPosition'])
            ->add('intHumidityPosition2', NumberType::class, ['label' => 'exposureSeriesForm.HumidityPosition2'])
            ->add('intHumidityAttenuation', NumberType::class, ['label' => 'exposureSeriesForm.HumidityAttenuation'])
            ->add('intHumidityAttenuation2', NumberType::class, ['label' => 'exposureSeriesForm.HumidityAttenuation2'])
            ->add('intHumidityDifference', NumberType::class, ['label' => 'exposureSeriesForm.HumidityDifference'])

            ->add('submit', SubmitType::class, [
                'label' => 'exposureSeriesForm.calculateBtn',
                'attr' => [
                    'class' => 'px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2'
                ]
            ])
            ->add('generate', SubmitType::class, [
                'label' => 'exposureSeriesForm.generateBtn',
                'attr' => [
                    'class' => 'px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2'
                ]
            ]);
//            ->add('weatherStation', EntityType::class, [
//                'class' => WeatherStation::class,
//                'choice_label' => 'id',
//            ])
        ;
    }

    public function configureOptions(OptionsResolver $resolver): void
    {
        $resolver->setDefaults([
            'data_class' => ExposureSeries::class,
        ]);
    }
}
