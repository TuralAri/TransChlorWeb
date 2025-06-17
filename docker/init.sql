INSERT INTO `aggregate_type` (`id`, `heat_capacity`, `name`, `aggregate_density`) VALUES
(1, -1, 'autre', -1),
(2, 0.74, 'Silex', 2625),
(3, 1, 'Gabbro', 2950),
(4, 0.9, 'Granite', 2650),
(5, 0.84, 'Calcaire', 2690),
(6, 0.9, 'Gravillon', 1650),
(7, 1, 'Coornéenne', 2900),
(8, 1, 'Prophyre', 2800),
(9, 0.79, 'Quartzite', 2650),
(10, 0.9, 'Schiste', 2700),
(11, 0.73, 'Siliceux', 2650),
(12, 0.89, 'Chaux+Calcaire', 2200);

--
-- Déchargement des données de la table `location`
--

INSERT INTO `location` (`id`, `name`, `co2`) VALUES
(1, 'autre', 0),
(2, 'centre ville', 0.036),
(3, 'zone industrielle', 0.045),
(4, 'campagne', 0.015);

--
-- Déchargement des données de la table `permeability`
--

INSERT INTO `permeability` (`id`, `d100_percent`, `dcl_to`, `heat_capacity`, `surface_transfer_coefficient`, `surface_heat_transfer`, `cement_density`, `ec`, `fresh_concrete_density`, `hydration_rate`, `air_content`, `ed`, `to_diffusion`, `alpha_diffusion`, `to_chloride_diffusion`, `name`) VALUES
(1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 'Other'),
(2, 0.0002, 0.00002, 0.7, 1, 1, 250, 0.73, 2387, 0.9, 1.5, 0, 293.16, 0, 293.16, 'CEM I 0.73'),
(3, 0.00013, 0.000013, 0.7, 1, 1, 375, 0.52, 2384, 0.9, 1.5, 0, 293.16, 0, 293.16, 'CEM I 0.52'),
(4, 0.00006, 0.000006, 0.7, 1, 1, 375, 0.42, 2450, 0.8, 1.5, 0, 293.16, 0, 293.16, 'CEM I 0.42');