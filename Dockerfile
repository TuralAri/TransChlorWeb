# Étape 1 : build des dépendances
FROM composer:2 as build

WORKDIR /app
COPY composer.json composer.lock ./
RUN composer install --no-dev --optimize-autoloader

COPY . .

# Étape 2 : PHP + Symfony sans les dev tools
FROM php:8.2-fpm-alpine

# Install extensions
RUN apk add --no-cache bash icu-dev libzip-dev libpng-dev libjpeg-turbo-dev freetype-dev libxml2-dev \
    && docker-php-ext-install intl pdo pdo_mysql zip opcache \
    && docker-php-ext-configure gd --with-freetype --with-jpeg \
    && docker-php-ext-install gd

WORKDIR /var/www/html

# Copier le code depuis le build stage
COPY --from=build /app ./

# Permissions sécurisées
RUN chown -R www-data:www-data /var/www/html \
    && chmod -R 755 /var/www/html

USER www-data

CMD ["php-fpm"]
