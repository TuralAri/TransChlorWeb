<?php
namespace App\Form;


use App\Entity\ImportFile;
use Symfony\Component\Form\AbstractType;
use Symfony\Component\Form\Extension\Core\Type\FileType;
use Symfony\Component\Form\FormBuilderInterface;
use Symfony\Component\OptionsResolver\OptionsResolver;

class ImportFileType extends AbstractType {



        public function buildForm(FormBuilderInterface $builder, array $options): void
    {
        $builder

            ->add("importFile", FileType::class, [
                'label' => 'Importer un fichier météo',
                'mapped' => false,
                'required' => true,
                'attr' => [
                    'class' => 'hidden',
                ]
            ]);

    }


    public function configureOptions(OptionsResolver $resolver): void
    {
        $resolver->setDefaults([
            'data_class' => ImportFile::class,
        ]);
    }
}