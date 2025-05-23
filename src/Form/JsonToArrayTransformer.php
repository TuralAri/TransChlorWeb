<?php

namespace App\Form;

use Symfony\Component\Form\DataTransformerInterface;
use Symfony\Component\Form\Exception\TransformationFailedException;

//This class permits to convert the string json array to a real array that can be saved in the entity
class JsonToArrayTransformer implements DataTransformerInterface
{
    public function transform($value): mixed
    {
        if (null === $value || $value === '') {
            return '';
        }
        return json_encode($value);
    }

    public function reverseTransform($value): mixed
    {
        if (null === $value || $value === '') {
            return [];
        }
        $data = json_decode($value, true);
        if (JSON_ERROR_NONE !== json_last_error()) {
            throw new TransformationFailedException('JSON invalide : ' . json_last_error_msg());
        }
        return $data;
    }
}