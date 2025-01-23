<?php

namespace App\Form;

use App\Entity\Meteo;
use Symfony\Component\Form\AbstractType;
use Symfony\Component\Form\FormBuilderInterface;
use Symfony\Component\OptionsResolver\OptionsResolver;
use Symfony\Component\Form\Extension\Core\Type\TextType;
use Symfony\Component\Form\Extension\Core\Type\IntegerType;

class MeteoType extends AbstractType
{
    public function buildForm(FormBuilderInterface $builder, array $options): void
    {
        $builder
            ->add('textBox1', TextType::class, [
                'label' => 'Text Box 1',
            ])
            ->add('textBox2', TextType::class, [
                'label' => 'Text Box 2',
            ])
            ->add('numericUpDown1', IntegerType::class, [
                'label' => 'Numeric Up Down 1',
            ])
            ->add('numericUpDown2', IntegerType::class, [
                'label' => 'Numeric Up Down 2',
            ]);
    }

    public function configureOptions(OptionsResolver $resolver): void
    {
        $resolver->setDefaults([
            'data_class' => Meteo::class,
        ]);
    }
}
