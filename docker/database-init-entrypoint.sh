#!/bin/sh

while ! mysql -h db -u symfony -psymfony -e "SELECT 1" > /dev/null 2>&1; do
    echo "En attente de la base de données..."
    sleep 1
done

php bin/console doctrine:database:create --if-not-exists

exec "$@"