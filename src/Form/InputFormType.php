<?php

namespace App\Form;

use App\Entity\Exposure;
use App\Entity\Input;
use App\Entity\Location;
use App\Entity\Material;
use App\Entity\ProbabilisticLawParams;
use Doctrine\DBAL\Types\JsonType;
use Symfony\Bridge\Doctrine\Form\Type\EntityType;
use Symfony\Component\Form\AbstractType;
use Symfony\Component\Form\Extension\Core\Type\ChoiceType;
use Symfony\Component\Form\Extension\Core\Type\SubmitType;
use Symfony\Component\Form\Extension\Core\Type\TextType;
use Symfony\Component\Form\FormBuilderInterface;
use Symfony\Component\OptionsResolver\OptionsResolver;

class InputFormType extends AbstractType
{
    public function buildForm(FormBuilderInterface $builder, array $options): void
    {
        $builder
            ->add('name')
            ->add('comment')
            ->add('saveTimeTemperature')
            ->add('saveTimeRelativeHumidity')
            ->add('saveTimeWaterContent')
            ->add('saveTimePh')
            ->add('saveTimeFreeChlorures')
            ->add('saveTimeTotalChlorures')
            ->add('maxComputingTime')
            ->add('computingTimeStep')
            ->add('wallThickness')
            ->add('elementsNumber')
            ->add('meshType', ChoiceType::class,[
                'choices' => [
                    '1: écart constant' => '1',
                    '2: écart proportionnel' =>'2',
                    '3: écart exponentiel'=>'3',
                    '4: Plusieurs écarts constants'=>'4',
                    '5: Plusieurs écarts constants, non symétriques' =>'5',
                ]
            ])
            ->add('resultsDisplayTime')
            ->add('capillarityTreatment', ChoiceType::class,[
                'choices' => [
                    'Usual capillary succion' => '1',
                    'Hydrophobic Treatment' => '2'
                ]
            ])
            ->add('leftEdgeCO2')
            ->add('rightEdgeCO2')
            ->add('thermalTransport')
            ->add('waterTransport')
            ->add('IonicTransport')
            ->add('isWaterVaporTransportActivated')
            ->add('isCapillarityTransportActivated')
            ->add('isIonicTransportActivated')
            ->add('isCarbonatationActivated')
            ->add('exposureFile1', EntityType::class, [
                'class' => Exposure::class,
                'choice_label' => 'type',
            ])
            ->add('exposureFile2', EntityType::class, [
                'class' => Exposure::class,
                'choice_label' => 'type',
            ])
            ->add('material', EntityType::class, [
                'class' => Material::class,
                'choice_label' => 'id',
                'multiple' => true,
            ])
            ->add('leftEdgeCO2Choice', EntityType::class, [
                'class' => Location::class,
                'choice_label' => 'name',
            ])
            ->add('rightEdgeCO2Choice', EntityType::class, [
                'class' => Location::class,
                'choice_label' => 'name',
            ])
            ->add('vaporWaterTransport', ProbabilisticLawFormType::class, [
//                'class' => ProbabilisticLawParams::class,
//                'choice_label' => 'id',
            ])
            ->add('liquidWaterTransportCapillarity', ProbabilisticLawFormType::class, [
//                'class' => ProbabilisticLawParams::class,
//                'choice_label' => 'id',
            ])
            ->add('chlorideIonicTransport', ProbabilisticLawFormType::class, [
//                'class' => ProbabilisticLawParams::class,
//                'choice_label' => 'id',
            ])
            ->add('carbonation', ProbabilisticLawFormType::class, [
//                'class' => ProbabilisticLawParams::class,
//                'choice_label' => 'id',
            ])
            ->add('submit', SubmitType::class);
        ;

        $builder->get('thermalTransport')->addModelTransformer(new JsonToArrayTransformer());
        $builder->get('waterTransport')->addModelTransformer(new JsonToArrayTransformer());
        $builder->get('IonicTransport')->addModelTransformer(new JsonToArrayTransformer());
    }

    public function configureOptions(OptionsResolver $resolver): void
    {
        $resolver->setDefaults([
            'data_class' => Input::class,
        ]);
    }
}
