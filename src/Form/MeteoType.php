<?php

namespace App\Form;

use App\Entity\Meteo;
use Symfony\Component\Form\AbstractType;
use Symfony\Component\Form\Extension\Core\Type\ButtonType;
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
            ->add("importFile", ButtonType::class, [
                'label' => 'Importer un fichier météo',
                'attr' => [
                    'class' => 'bg-blue-500 text-white px-4 py-2 rounded-full hover:bg-blue-800'
                ]
            ])
            ->add('fileYears', NumberType::class, ['label' => 'Nombre d\'années (fichier)'])
            ->add('sodiumChlorideConcentration', NumberType::class, ['label' => 'Concentration de chlorure de sodium au temps'])
            ->add('waterFilmThickness', NumberType::class, ['label' => 'Épaisseur du film d\'eau sur la chaussée'])
            ->add('humidityThreshold', NumberType::class, ['label' => 'Humidité relative seuil d\'intervention en cas de basses températures'])

            ->add('mechanicalAnnualSodium', NumberType::class, ['label' => 'Concentration annuelle en chlorure de sodium'])
            ->add('mechanicalMeanSodium', NumberType::class, ['label' => 'Quantité moyenne de chlorure de sodium'])
            ->add('mechanicalInterventions', NumberType::class, ['label' => 'Nombre d\'interventions d\'épandage de chlorure de sodium'])
            ->add('mechanicalInterval', NumberType::class, ['label' => 'Intervalle minimal entre épandages'])
            ->add('mechanicalSodiumWater', NumberType::class, ['label' => 'Concentration de chlorure de sodium dans l\'eau'])
            ->add('mechanicalThresholdTemperature', NumberType::class, ['label' => 'Température seuil d\'intervention', 'required' => false])

            ->add('automaticAnnualSodium', NumberType::class, ['label' => 'Concentration annuelle en chlorure de sodium'])
            ->add('automaticMeanSodium', NumberType::class, ['label' => 'Quantité moyenne de chlorure de sodium'])
            ->add('automaticSprays', NumberType::class, ['label' => 'Nombre de giclages annuels'])
            ->add('automaticSprayInterval', NumberType::class, ['label' => 'Nombre de giclages sur un intervalle de temps'])
            ->add('automaticSodiumWater', NumberType::class, ['label' => 'Concentration de chlorure de sodium dans l\'eau (épandage) Si épandage solide (100%)'])
            ->add('automaticThresholdTemperature', NumberType::class, ['label' => 'Température seuil d\'intervention', 'required' => false])

            ->add('extTemperaturePosition', NumberType::class, ['label' => 'Position de la moyenne au'])
            ->add('extTemperaturePosition2', NumberType::class, ['label' => '/'])
            ->add('extTemperatureAttenuation', NumberType::class, ['label' => 'Atténuation de'])
            ->add('extTemperatureAttenuation2', NumberType::class, ['label' => '/'])
            ->add('extTemperatureDifference', NumberType::class, ['label' => 'Différence de température limite'])

            ->add('extHumidityPosition', NumberType::class, ['label' => 'Position de la moyenne au'])
            ->add('extHumidityPosition2', NumberType::class, ['label' => '/'])
            ->add('extHumidityAttenuation', NumberType::class, ['label' => 'Atténuation de'])
            ->add('extHumidityAttenuation2', NumberType::class, ['label' => '/'])
            ->add('extHumidityDifference', NumberType::class, ['label' => 'Différence Humidité relative limite'])

            ->add('intTemperaturePosition', NumberType::class, ['label' => 'Position de la moyenne au'])
            ->add('intTemperaturePosition2', NumberType::class, ['label' => '/'])
            ->add('intTemperatureAttenuation', NumberType::class, ['label' => 'Atténuation de'])
            ->add('intTemperatureAttenuation2', NumberType::class, ['label' => '/'])
            ->add('intTemperatureDifference', NumberType::class, ['label' => 'Différence de température limite'])

            ->add('intHumidityPosition', NumberType::class, ['label' => 'Position de la moyenne au'])
            ->add('intHumidityPosition2', NumberType::class, ['label' => '/'])
            ->add('intHumidityAttenuation', NumberType::class, ['label' => 'Atténuation de'])
            ->add('intHumidityAttenuation2', NumberType::class, ['label' => '/'])
            ->add('intHumidityDifference', NumberType::class, ['label' => 'Différence Humidité relative limite'])


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
