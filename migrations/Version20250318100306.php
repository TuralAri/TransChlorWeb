<?php

declare(strict_types=1);

namespace DoctrineMigrations;

use Doctrine\DBAL\Schema\Schema;
use Doctrine\Migrations\AbstractMigration;

/**
 * Auto-generated Migration: Please modify to your needs!
 */
final class Version20250318100306 extends AbstractMigration
{
    public function getDescription(): string
    {
        return '';
    }

    public function up(Schema $schema): void
    {
        // this up() migration is auto-generated, please modify it to your needs
        $this->addSql('CREATE TABLE meteo (id INT AUTO_INCREMENT NOT NULL, file_years DOUBLE PRECISION NOT NULL, sodium_chloride_concentration DOUBLE PRECISION NOT NULL, water_film_thickness DOUBLE PRECISION NOT NULL, humidity_threshold DOUBLE PRECISION NOT NULL, mechanical_annual_sodium DOUBLE PRECISION NOT NULL, mechanical_mean_sodium DOUBLE PRECISION NOT NULL, mechanical_interventions DOUBLE PRECISION NOT NULL, mechanical_interval DOUBLE PRECISION NOT NULL, mechanical_sodium_water DOUBLE PRECISION NOT NULL, mechanical_threshold_temperature DOUBLE PRECISION DEFAULT NULL, automatic_annual_sodium DOUBLE PRECISION NOT NULL, automatic_mean_sodium DOUBLE PRECISION NOT NULL, automatic_sprays DOUBLE PRECISION NOT NULL, automatic_spray_interval DOUBLE PRECISION NOT NULL, automatic_sodium_water DOUBLE PRECISION NOT NULL, automatic_threshold_temperature DOUBLE PRECISION DEFAULT NULL, ext_temperature_position DOUBLE PRECISION NOT NULL, ext_temperature_position2 DOUBLE PRECISION NOT NULL, ext_temperature_attenuation DOUBLE PRECISION NOT NULL, ext_temperature_attenuation2 DOUBLE PRECISION NOT NULL, ext_temperature_difference DOUBLE PRECISION NOT NULL, ext_humidity_position DOUBLE PRECISION NOT NULL, ext_humidity_position2 DOUBLE PRECISION NOT NULL, ext_humidity_attenuation DOUBLE PRECISION NOT NULL, ext_humidity_attenuation2 DOUBLE PRECISION NOT NULL, ext_humidity_difference DOUBLE PRECISION NOT NULL, int_temperature_position DOUBLE PRECISION NOT NULL, int_temperature_position2 DOUBLE PRECISION NOT NULL, int_temperature_attenuation DOUBLE PRECISION NOT NULL, int_temperature_attenuation2 DOUBLE PRECISION NOT NULL, int_temperature_difference DOUBLE PRECISION NOT NULL, int_humidity_position DOUBLE PRECISION NOT NULL, int_humidity_position2 DOUBLE PRECISION NOT NULL, int_humidity_attenuation DOUBLE PRECISION NOT NULL, int_humidity_attenuation2 DOUBLE PRECISION NOT NULL, int_humidity_difference DOUBLE PRECISION NOT NULL, PRIMARY KEY(id)) DEFAULT CHARACTER SET utf8mb4 COLLATE `utf8mb4_unicode_ci` ENGINE = InnoDB');
    }

    public function down(Schema $schema): void
    {
        // this down() migration is auto-generated, please modify it to your needs
        $this->addSql('DROP TABLE meteo');
    }
}
