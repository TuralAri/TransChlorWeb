FROM php:8.2-apache

ARG HTTP_PROXY
ARG HTTPS_PROXY

ENV http_proxy=$HTTP_PROXY
ENV https_proxy=$HTTPS_PROXY

# Activer les modules Apache nécessaires
RUN a2enmod rewrite
RUN a2enmod ssl

COPY docker/apache.conf /etc/apache2/sites-available/000-default.conf
# Installer les extensions PHP nécessaires pour Symfony
RUN apt-get update && apt-get install -y \
    libicu-dev libonig-dev libzip-dev zip unzip git curl \
    && docker-php-ext-install intl pdo pdo_mysql zip opcache

# Installer les dépendances de Node.js et NPM
RUN curl -fsSL https://deb.nodesource.com/setup_18.x | bash - \
    && apt-get install -y nodejs \
    && npm install -g npm

# Installer Composer
COPY --from=composer:latest /usr/bin/composer /usr/bin/composer

# Copier le code Symfony
COPY . /var/www/html/

# Définir les droits
RUN chown -R www-data:www-data /var/www/html

# Changer le docroot pour Symfony (dossier `public`)
ENV APACHE_DOCUMENT_ROOT /var/www/html/public

WORKDIR /var/www/html
RUN composer install --no-interaction --optimize-autoloader \
    && npm install \
    && npm run build