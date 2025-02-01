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
            ->add('mechanicalInterventions', IntegerType::class, ['label' => 'Nombre d\'interventions d\'épandage de chlorure de sodium'])
            ->add('mechanicalInterval', NumberType::class, ['label' => 'Intervalle minimal entre épandages'])
            ->add('mechanicalSodiumWater', IntegerType::class, ['label' => 'Concentration de chlorure de sodium dans l\'eau'])
            ->add('mechanicalThresholdTemperature', NumberType::class, ['label' => 'Température seuil d\'intervention', 'required' => false])

            ->add('automaticAnnualSodium', NumberType::class, ['label' => 'Concentration annuelle en chlorure de sodium'])
            ->add('automaticMeanSodium', NumberType::class, ['label' => 'Quantité moyenne de chlorure de sodium'])
            ->add('automaticSprays', IntegerType::class, ['label' => 'Nombre de giclages annuels'])
            ->add('automaticSprayInterval', IntegerType::class, ['label' => 'Nombre de giclages sur un intervalle de temps'])
            ->add('automaticSodiumWater', IntegerType::class, ['label' => 'Concentration de chlorure de sodium dans l\'eau (épandage) Si épandage solide (100%)'])
            ->add('automaticThresholdTemperature', NumberType::class, ['label' => 'Température seuil d\'intervention', 'required' => false])

            ->add('extTemperaturePosition', IntegerType::class, ['label' => 'Position de la moyenne au'])
            ->add('extTemperaturePosition2', IntegerType::class, ['label' => '/'])
            ->add('extTemperatureAttenuation', IntegerType::class, ['label' => 'Atténuation de'])
            ->add('extTemperatureAttenuation2', IntegerType::class, ['label' => '/'])
            ->add('extTemperatureDifference', IntegerType::class, ['label' => 'Différence de température limite'])

            ->add('extHumidityPosition', IntegerType::class, ['label' => 'Position de la moyenne au'])
            ->add('extHumidityPosition2', IntegerType::class, ['label' => '/'])
            ->add('extHumidityAttenuation', IntegerType::class, ['label' => 'Atténuation de'])
            ->add('extHumidityAttenuation2', IntegerType::class, ['label' => '/'])
            ->add('extHumidityDifference', IntegerType::class, ['label' => 'Différence Humidité relative limite'])

            ->add('intTemperaturePosition', IntegerType::class, ['label' => 'Position de la moyenne au'])
            ->add('intTemperaturePosition2', IntegerType::class, ['label' => '/'])
            ->add('intTemperatureAttenuation', IntegerType::class, ['label' => 'Atténuation de'])
            ->add('intTemperatureAttenuation2', IntegerType::class, ['label' => '/'])
            ->add('intTemperatureDifference', IntegerType::class, ['label' => 'Différence de température limite'])

            ->add('intHumidityPosition', IntegerType::class, ['label' => 'Position de la moyenne au'])
            ->add('intHumidityPosition2', IntegerType::class, ['label' => '/'])
            ->add('intHumidityAttenuation', IntegerType::class, ['label' => 'Atténuation de'])
            ->add('intHumidityAttenuation2', IntegerType::class, ['label' => '/'])
            ->add('intHumidityDifference', IntegerType::class, ['label' => 'Différence Humidité relative limite'])


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
