<?php
namespace App\Form;


use App\Entity\ExportFile;
use Symfony\Component\Form\AbstractType;
use Symfony\Component\Form\Extension\Core\Type\SubmitType;
use Symfony\Component\Form\FormBuilderInterface;
use Symfony\Component\OptionsResolver\OptionsResolver;

class ExportFileType extends AbstractType {



    public function buildForm(FormBuilderInterface $builder, array $options): void
    {
        $builder
            ->add('exportFile', SubmitType::class, [
                'label' => 'Exporter un fichier météo',
                'attr' => ['class' => 'hidden'],
            ]);
    }


    public function configureOptions(OptionsResolver $resolver): void
    {
        $resolver->setDefaults([
            'data_class' => ExportFile::class,
        ]);
    }
}