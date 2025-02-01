<?php

namespace App\Form;

use App\Entity\Meteo;
use Symfony\Component\Form\AbstractType;
use Symfony\Component\Form\FormBuilderInterface;
use Symfony\Component\OptionsResolver\OptionsResolver;
use Symfony\Component\Form\Extension\Core\Type\NumberType;
use Symfony\Component\Form\Extension\Core\Type\IntegerType;
use Symfony\Component\Form\Extension\Core\Type\SubmitType;

class MeteoType extends AbstractType
{
    public function buildForm(FormBuilderInterface $builder, array $options): void
    {
        $builder
            ->add('fileYears', NumberType::class, ['label' => 'Nombre d\'années (fichier)'])
            ->add('sodiumChlorideConcentration', NumberType::class, ['label' => 'Concentration de chlorure de sodium au temps'])
            ->add('waterFilmThickness', NumberType::class, ['label' => 'Épaisseur du film d\'eau sur la chaussée'])
            ->add('humidityThreshold', IntegerType::class, ['label' => 'Humidité relative seuil d\'intervention en cas de basses températures'])

            ->add('mechanicalAnnualSodium', NumberType::class, ['label' => 'Concentration annuelle en chlorure de sodium'])
            ->add('mechanicalMeanSodium', NumberType::class, ['label' => 'Quantité moyenne de chlorure de sodium'])
            ->add('mechanicalInterventions', IntegerType::class, ['label' => 'Nombre d\'interventions d\'épandage de chlorure de sodium (mécanique)'])
            ->add('mechanicalInterval', NumberType::class, ['label' => 'Intervalle minimal entre épandages (mécanique)'])
            ->add('mechanicalSodiumWater', IntegerType::class, ['label' => 'Concentration de chlorure de sodium dans l\'eau (mécanique)'])
            ->add('mechanicalThresholdTemperature', NumberType::class, ['label' => 'Température seuil d\'intervention (mécanique)', 'required' => false])

            ->add('automaticAnnualSodium', NumberType::class, ['label' => 'Concentration annuelle en chlorure de sodium (automatique)'])
            ->add('automaticMeanSodium', NumberType::class, ['label' => 'Quantité moyenne de chlorure de sodium (automatique)'])
            ->add('automaticSprays', IntegerType::class, ['label' => 'Nombre de giclages annuels (automatique)'])
            ->add('automaticSprayInterval', IntegerType::class, ['label' => 'Nombre de giclages sur un intervalle de temps (automatique)'])
            ->add('automaticSodiumWater', IntegerType::class, ['label' => 'Concentration de chlorure de sodium dans l\'eau (automatique)'])
            ->add('automaticThresholdTemperature', NumberType::class, ['label' => 'Température seuil d\'intervention (automatique)', 'required' => false])

            ->add('extTemperaturePosition', IntegerType::class, ['label' => 'Position Température extérieure'])
            ->add('extTemperatureDivider', IntegerType::class, ['label' => 'Diviseur Température extérieure'])
            ->add('extTemperatureAttenuation', IntegerType::class, ['label' => 'Atténuation Température extérieure'])
            ->add('extTemperatureLimit', NumberType::class, ['label' => 'Limite Température extérieure'])

            ->add('extHumidityPosition', IntegerType::class, ['label' => 'Position Humidité extérieure'])
            ->add('extHumidityDivider', IntegerType::class, ['label' => 'Diviseur Humidité extérieure'])
            ->add('extHumidityAttenuation', IntegerType::class, ['label' => 'Atténuation Humidité extérieure'])
            ->add('extHumidityLimit', NumberType::class, ['label' => 'Limite Humidité extérieure'])

            ->add('intTemperaturePosition', IntegerType::class, ['label' => 'Position Température intérieure'])
            ->add('intTemperatureDivider', IntegerType::class, ['label' => 'Diviseur Température intérieure'])
            ->add('intTemperatureAttenuation', IntegerType::class, ['label' => 'Atténuation Température intérieure'])
            ->add('intTemperatureLimit', NumberType::class, ['label' => 'Limite Température intérieure'])

            ->add('intHumidityPosition', IntegerType::class, ['label' => 'Position Humidité intérieure'])
            ->add('intHumidityDivider', IntegerType::class, ['label' => 'Diviseur Humidité intérieure'])
            ->add('intHumidityAttenuation', IntegerType::class, ['label' => 'Atténuation Humidité intérieure'])
            ->add('intHumidityLimit', NumberType::class, ['label' => 'Limite Humidité intérieure'])


            ->add('submit', SubmitType::class, [
                'label' => 'Calculer',
                'attr' => [
                    'class' => 'px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2'
                ]
                ]);
    }

    public function configureOptions(OptionsResolver $resolver): void
    {
        $resolver->setDefaults([
            'data_class' => Meteo::class,
        ]);
    }
}
