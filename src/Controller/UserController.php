<?php

namespace App\Controller;

use App\Entity\User;
use Symfony\Bundle\FrameworkBundle\Controller\AbstractController;
use Symfony\Component\HttpFoundation\Response;
use Symfony\Component\PasswordHasher\Hasher\UserPasswordHasherInterface;
use Symfony\Component\Routing\Annotation\Route;
use Doctrine\Persistence\ManagerRegistry;
use App\Repository\UserRepository;



class UserController extends AbstractController
{
    #[Route(path: '/insert-first-user-into-database', name: 'insert_first_user')]
    public function insertFirstUser(UserPasswordHasherInterface $passwordHasher, ManagerRegistry $doctrine, UserRepository $userRepository): Response
    {
        $existingUser = $userRepository->findOneBy(['username' => 'first_user']);

        if ($existingUser) {
            return new Response('User already exists');
        }

        $entityManager = $doctrine->getManager();
        $user = new User();
        $user->setUsername('first_user');
        $hashedPassword = $passwordHasher->hashPassword($user, 'first_user123');
        $user->setPassword($hashedPassword);
        $user->setRoles(['ROLE_ADMIN']);
        $entityManager->persist($user);
        $entityManager->flush();

        return new Response('First user inserted');
    }
}