<?php

namespace App\Form;

use App\Entity\Exposure;
use App\Entity\ExposureSeries;
use App\Entity\Input;
use App\Entity\Location;
use App\Entity\Material;
use App\Entity\ProbabilisticLawParams;
use App\Entity\WeatherStation;
use Doctrine\DBAL\Types\JsonType;
use Symfony\Bridge\Doctrine\Form\Type\EntityType;
use Symfony\Component\Form\AbstractType;
use Symfony\Component\Form\Extension\Core\Type\ChoiceType;
use Symfony\Component\Form\Extension\Core\Type\SubmitType;
use Symfony\Component\Form\Extension\Core\Type\TextareaType;
use Symfony\Component\Form\Extension\Core\Type\TextType;
use Symfony\Component\Form\FormBuilderInterface;
use Symfony\Component\Form\FormEvent;
use Symfony\Component\Form\FormEvents;
use Symfony\Component\OptionsResolver\OptionsResolver;
use Symfony\Component\Security\Core\User\UserInterface;

class InputFormType extends AbstractType
{
    public function buildForm(FormBuilderInterface $builder, array $options): void
    {
        /** @var UserInterface $user */
        $user = $options['user'];

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
            ->add('edgeElementLength')
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
            ->add('weatherStation', EntityType::class, [
                'class' => WeatherStation::class,
//                'choice_label' => 'id' . 'localFileName',
                'placeholder' => 'Choisissez une station météo',
                'mapped' => false,
                'required' => true,
            ])
            ->add('exposureSeries', EntityType::class, [
                'class' => ExposureSeries::class,
                'choice_label' => 'name',
                'placeholder' => 'Choisissez une série d’exposition',
                'mapped' => false,
                'required' => true,
                'choices' => [], // AJAX will fill this
            ])
            ->add('exposureFile1', EntityType::class, [
                'class' => Exposure::class,
                'choice_label' => 'type',
                'placeholder' => 'Choisissez une exposition',
                'choices' => [], // AJAX will fill this
            ])
            ->add('exposureFile2', EntityType::class, [
                'class' => Exposure::class,
                'choice_label' => 'type',
                'placeholder' => 'Choisissez une exposition',
                'choices' => [], // AJAX will fill this
            ])
            ->add('material', EntityType::class, [
                'class' => Material::class,
                'choice_label' => 'name',
                'multiple' => true,
                'query_builder' => function ($repo) use ($user, $builder) {
                    // adds the already existing materials for the user
                    $input = $builder->getData();
                    $qb = $repo->createQueryBuilder('m')
                        ->where('m.user = :user')
                        ->setParameter('user', $user);

                    if ($input && count($input->getMaterial()) > 0) {
                        $qb->orWhere('m.id IN (:materials)')
                            ->setParameter('materials', $input->getMaterial());
                    }

                    return $qb;
                },
            ])
            ->add('leftEdgeCO2Choice', EntityType::class, [
                'class' => Location::class,
                'choice_label' => 'name',
            ])
            ->add('rightEdgeCO2Choice', EntityType::class, [
                'class' => Location::class,
                'choice_label' => 'name',
            ])
            ->add('aoDiffusion', TextType::class, [
                'label' => 'materialForm.labels.aoDiffusion',
            ])
            ->add('hc', TextType::class, [
                'label' => 'materialForm.labels.hc',
            ])
            ->add('aoCapillarity', TextType::class, [
                'label' => 'materialForm.labels.aoCapillarity',
            ])
            ->add('tc', TextType::class, [
                'label' => 'materialForm.labels.tc',
            ])
            ->add('limitWaterContent', TextType::class, [
                'label' => 'materialForm.labels.limitWaterContent',
            ])
            ->add('delayCoefficient', TextType::class, [
                'label' => 'materialForm.labels.retardationCoefficient',
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
            ->add('adsorptionFa', TextType::class, [
                'label' => 'materialForm.labels.adsorptionFa',
            ])
            ->add('heatCapacity', TextType::class, [
                'label' => 'materialForm.labels.heatCapacity',
            ])
            ->add('probabilisticData', TextareaType::class, [
                'mapped' => false,
                'required' => false,
//                'attr' => ['style' => 'display:none;'], // champ caché
            ])
            ->add('submit', SubmitType::class);
        ;

        //adds listeners to dynamically update the form based on previous selections
        $builder->addEventListener(FormEvents::PRE_SUBMIT, function (FormEvent $event) {
            $form = $event->getForm();
            $data = $event->getData();
            $em = $form->getConfig()->getOption('em');

            // Pour exposureSeries
            if (isset($data['weatherStation'])) {
                $series = $em->getRepository(ExposureSeries::class)->findBy(['weatherStation' => $data['weatherStation']]);
                $form->add('exposureSeries', EntityType::class, [
                    'class' => ExposureSeries::class,
                    'choices' => $series,
                    'choice_label' => 'name',
                    'placeholder' => 'Choisissez une série d’exposition',
                    'mapped' => false,
                    'required' => true,
                ]);
            }

            // Pour exposureFile1 et exposureFile2
            if (isset($data['exposureSeries'])) {
                $expos = $em->getRepository(Exposure::class)->findBy(['ExposureSerie' => $data['exposureSeries']]);
                $form->add('exposureFile1', EntityType::class, [
                    'class' => Exposure::class,
                    'choices' => $expos,
                    'choice_label' => 'type',
                    'placeholder' => 'Choisissez une exposition',
                    'required' => true,
                ]);
                $form->add('exposureFile2', EntityType::class, [
                    'class' => Exposure::class,
                    'choices' => $expos,
                    'choice_label' => 'type',
                    'placeholder' => 'Choisissez une exposition',
                    'required' => true,
                ]);
            }
        });

        // added transformers for JSON fields
        $builder->get('thermalTransport')->addModelTransformer(new JsonToArrayTransformer());
        $builder->get('waterTransport')->addModelTransformer(new JsonToArrayTransformer());
        $builder->get('IonicTransport')->addModelTransformer(new JsonToArrayTransformer());
    }

    public function configureOptions(OptionsResolver $resolver): void
    {
        $resolver->setDefaults([
            'data_class' => Input::class,
            'user' => null,
            'em' => null, // EntityManager for fetching exposures
        ]);
    }
}
