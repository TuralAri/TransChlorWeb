<?php

namespace App\Form;

use App\Entity\Material;
use App\Entity\ProbabilisticLawParams;
use Symfony\Bridge\Doctrine\Form\Type\EntityType;
use Symfony\Component\Form\AbstractType;
use Symfony\Component\Form\Extension\Core\Type\ChoiceType;
use Symfony\Component\Form\Extension\Core\Type\TextareaType;
use Symfony\Component\Form\Extension\Core\Type\TextType;
use Symfony\Component\Form\FormBuilderInterface;
use Symfony\Component\OptionsResolver\OptionsResolver;

class ProbabilisticLawFormType extends AbstractType
{
    public function buildForm(FormBuilderInterface $builder, array $options): void
    {
        $commonAttr = ['class' => 'ml-2 p-2 border rounded-lg text-left'];

        $builder
            ->add('material', EntityType::class, [
                'class' => Material::class,
                'choice_label' => 'name',
                'attr' => ['class' => 'hidden'],
                'label_attr' => ['class' => 'hidden'],
            ])
            ->add('type', TextType::class, [
                'attr' => ['class' => 'hidden'],
                'label_attr' => ['class' => 'hidden'],
                'required' => false,
            ])
            ->add('lawType', ChoiceType::class,[
                'attr' => $commonAttr,
                'choices' => [
                    'Loi normale' => '1',
                    'Loi logonormale' =>'2',
                ]
            ])
            ->add('meanValue', TextType::class, [
                'attr' => $commonAttr,
                'required' => false,
                'empty_data' => null,
            ])
            ->add('standardDeviation', TextType::class, [
                'attr' => $commonAttr,
                'required' => false,
                'empty_data' => null,
            ])
            ->add('lambda', TextType::class, [
                'attr' => $commonAttr,
                'required' => false,
                'empty_data' => null,
            ])
            ->add('ksi', TextType::class, [
                'attr' => $commonAttr,
                'required' => false,
                'empty_data' => null,
            ])
            ->add('pMinus', TextType::class, [
                'attr' => $commonAttr,
                'required' => false,
                'empty_data' => null,
            ])
            ->add('pPlus', TextType::class, [
                'attr' => $commonAttr,
                'required' => false,
                'empty_data' => null,
            ])
            ->add('x1', TextType::class, [
                'attr' => $commonAttr,
                'required' => false,
                'empty_data' => null,
            ])
            ->add('x2', TextType::class, [
                'attr' => $commonAttr,
                'required' => false,
                'empty_data' => null,
            ])
        ;
    }

    public function configureOptions(OptionsResolver $resolver): void
    {
        $resolver->setDefaults([
            'data_class' => ProbabilisticLawParams::class,
        ]);
    }
}
