<?php

namespace App\EventSubscriber;

use Symfony\Component\EventDispatcher\EventSubscriberInterface;
use Symfony\Component\HttpKernel\Event\RequestEvent;
use Symfony\Component\HttpKernel\KernelEvents;

class LocaleSubscriber implements EventSubscriberInterface
{
    private string $defaultLocale;
    public function __construct(string $defaultLocale = "fr"){
        $this->defaultLocale = $defaultLocale;
    }

    public function onKernelRequest(RequestEvent $event): void{
        $request = $event->getRequest();
        $session = $request->getSession();

        if($request->query->has("_locale")){
            $session->set("_locale", $request->query->get("_locale"));
        }

        $locale = $session->get("_locale", $this->defaultLocale);
        $request->setLocale($locale);
    }

    public static function getSubscribedEvents(): array{
        return [
            KernelEvents::REQUEST => [['onKernelRequest', 20]],
        ];
    }
}