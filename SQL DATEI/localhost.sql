-- phpMyAdmin SQL Dump
-- version 5.2.2
-- https://www.phpmyadmin.net/
--
-- Host: localhost
-- Erstellungszeit: 06. Mrz 2026 um 09:26
-- Server-Version: 11.8.3-MariaDB-0+deb13u1 from Debian
-- PHP-Version: 8.4.16

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Datenbank: `pbt3h24akr_testdb`
--
CREATE DATABASE IF NOT EXISTS `pbt3h24akr_testdb` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_uca1400_ai_ci;
USE `pbt3h24akr_testdb`;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `Buchung`
--

CREATE TABLE `Buchung` (
  `BuchungId` int(11) NOT NULL,
  `TeilnehmerId` int(11) NOT NULL,
  `SeminarId` int(11) NOT NULL,
  `Buchungsdatum` datetime DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;

--
-- Daten für Tabelle `Buchung`
--

INSERT INTO `Buchung` (`BuchungId`, `TeilnehmerId`, `SeminarId`, `Buchungsdatum`) VALUES
(1, 1, 3, '2025-11-26 11:08:33'),
(2, 1, 2, '2025-11-26 11:08:48'),
(3, 1, 1, '2025-11-26 11:13:06'),
(4, 1, 3, '2025-12-03 09:21:22'),
(5, 1, 1, '2025-12-03 09:44:10'),
(6, 1, 3, '2025-12-03 10:12:58'),
(7, 1, 3, '2025-12-04 09:34:54'),
(8, 1, 3, '2025-12-04 09:34:58'),
(9, 1, 3, '2025-12-04 09:35:00');

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `Seminar`
--

CREATE TABLE `Seminar` (
  `SeminarId` int(11) NOT NULL,
  `Titel` varchar(100) NOT NULL,
  `Datum` date NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;

--
-- Daten für Tabelle `Seminar`
--

INSERT INTO `Seminar` (`SeminarId`, `Titel`, `Datum`) VALUES
(1, 'Einführung in C#', '2025-12-10'),
(2, 'Datenbanken mit MySQL', '2025-12-15'),
(3, 'WPF Grundlagen', '2025-12-20');

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `Teilnehmer`
--

CREATE TABLE `Teilnehmer` (
  `TeilnehmerId` int(11) NOT NULL,
  `Vorname` varchar(100) NOT NULL,
  `Nachname` varchar(100) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;

--
-- Daten für Tabelle `Teilnehmer`
--

INSERT INTO `Teilnehmer` (`TeilnehmerId`, `Vorname`, `Nachname`) VALUES
(1, 'Anna', 'Müller'),
(2, 'Peter', 'Schmidt'),
(3, 'Julia', 'Becker'),
(4, 'Max', 'Wagner'),
(5, 'Laura', 'Klein');

--
-- Indizes der exportierten Tabellen
--

--
-- Indizes für die Tabelle `Buchung`
--
ALTER TABLE `Buchung`
  ADD PRIMARY KEY (`BuchungId`),
  ADD KEY `TeilnehmerId` (`TeilnehmerId`),
  ADD KEY `SeminarId` (`SeminarId`);

--
-- Indizes für die Tabelle `Seminar`
--
ALTER TABLE `Seminar`
  ADD PRIMARY KEY (`SeminarId`);

--
-- Indizes für die Tabelle `Teilnehmer`
--
ALTER TABLE `Teilnehmer`
  ADD PRIMARY KEY (`TeilnehmerId`);

--
-- AUTO_INCREMENT für exportierte Tabellen
--

--
-- AUTO_INCREMENT für Tabelle `Buchung`
--
ALTER TABLE `Buchung`
  MODIFY `BuchungId` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=10;

--
-- AUTO_INCREMENT für Tabelle `Seminar`
--
ALTER TABLE `Seminar`
  MODIFY `SeminarId` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT für Tabelle `Teilnehmer`
--
ALTER TABLE `Teilnehmer`
  MODIFY `TeilnehmerId` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- Constraints der exportierten Tabellen
--

--
-- Constraints der Tabelle `Buchung`
--
ALTER TABLE `Buchung`
  ADD CONSTRAINT `Buchung_ibfk_1` FOREIGN KEY (`TeilnehmerId`) REFERENCES `Teilnehmer` (`TeilnehmerId`) ON DELETE CASCADE ON UPDATE CASCADE,
  ADD CONSTRAINT `Buchung_ibfk_2` FOREIGN KEY (`SeminarId`) REFERENCES `Seminar` (`SeminarId`) ON DELETE CASCADE ON UPDATE CASCADE;
--
-- Datenbank: `pbt3h24akr_Wombank`
--
CREATE DATABASE IF NOT EXISTS `pbt3h24akr_Wombank` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_uca1400_ai_ci;
USE `pbt3h24akr_Wombank`;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `accounts`
--

CREATE TABLE `accounts` (
  `account_id` int(11) NOT NULL,
  `user_id` int(11) NOT NULL,
  `iban` varchar(34) NOT NULL,
  `balance` decimal(12,2) DEFAULT 0.00,
  `currency` char(3) DEFAULT 'EUR',
  `status` enum('OPEN','FROZEN','CLOSED') DEFAULT 'OPEN',
  `created_at` timestamp NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;

--
-- Daten für Tabelle `accounts`
--

INSERT INTO `accounts` (`account_id`, `user_id`, `iban`, `balance`, `currency`, `status`, `created_at`) VALUES
(1, 1, 'DE44500105175407324931', 15000.00, 'EUR', 'OPEN', '2026-02-12 10:01:19'),
(2, 2, 'DE12500105170648489890', 2350.50, 'EUR', 'OPEN', '2026-02-12 10:01:19'),
(3, 3, 'DE21500105170648481234', 13020.00, 'EUR', 'OPEN', '2026-02-12 10:01:19'),
(4, 8, 'DE44500105175407324999', 82735890.67, 'EUR', 'OPEN', '2026-02-25 09:27:39');

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `beneficiaries`
--

CREATE TABLE `beneficiaries` (
  `beneficiary_id` int(11) NOT NULL,
  `user_id` int(11) NOT NULL,
  `name` varchar(100) NOT NULL,
  `iban` varchar(34) NOT NULL,
  `bic` varchar(11) DEFAULT NULL,
  `nickname` varchar(50) DEFAULT NULL,
  `created_at` timestamp NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;

--
-- Daten für Tabelle `beneficiaries`
--

INSERT INTO `beneficiaries` (`beneficiary_id`, `user_id`, `name`, `iban`, `bic`, `nickname`, `created_at`) VALUES
(1, 2, 'Anna Schmidt', 'DE21500105170648481234', 'INGDDEFFXXX', 'Anna', '2026-02-12 10:01:19'),
(2, 3, 'Max Mustermann', 'DE12500105170648489890', 'COBADEFFXXX', 'Max Privat', '2026-02-12 10:01:19');

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `cards`
--

CREATE TABLE `cards` (
  `card_id` int(11) NOT NULL,
  `account_id` int(11) NOT NULL,
  `card_token` varchar(255) NOT NULL,
  `last4` char(4) NOT NULL,
  `brand` enum('VISA','MASTERCARD','MAESTRO','AMEX','OTHER') DEFAULT 'OTHER',
  `exp_month` tinyint(4) DEFAULT NULL,
  `exp_year` smallint(6) DEFAULT NULL,
  `status` enum('ACTIVE','BLOCKED','EXPIRED') DEFAULT 'ACTIVE',
  `created_at` timestamp NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;

--
-- Daten für Tabelle `cards`
--

INSERT INTO `cards` (`card_id`, `account_id`, `card_token`, `last4`, `brand`, `exp_month`, `exp_year`, `status`, `created_at`) VALUES
(1, 1, 'tok_acc1_card1_7f3a91', '4821', 'VISA', 12, 2028, 'ACTIVE', '2026-02-25 09:18:59'),
(2, 1, 'tok_acc1_card2_9a8c44', '1934', 'MASTERCARD', 6, 2027, 'ACTIVE', '2026-02-25 09:18:59'),
(3, 2, 'tok_acc2_card1_3bd882', '7750', 'VISA', 3, 2029, 'ACTIVE', '2026-02-25 09:18:59'),
(4, 2, 'tok_acc2_card2_4fa991', '6612', 'MASTERCARD', 11, 2026, 'ACTIVE', '2026-02-25 09:18:59'),
(5, 3, 'tok_acc3_card1_8de221', '9045', 'VISA', 9, 2028, 'ACTIVE', '2026-02-25 09:18:59'),
(6, 3, 'tok_acc3_card2_1cc734', '3187', 'MAESTRO', 1, 2027, 'ACTIVE', '2026-02-25 09:18:59'),
(7, 4, 'tok_test8_card1_a91xk2', '1123', 'VISA', 5, 2029, 'ACTIVE', '2026-02-25 09:28:24'),
(8, 4, 'tok_test8_card2_b72pl9', '4488', 'MASTERCARD', 8, 2027, 'ACTIVE', '2026-02-25 09:28:24'),
(9, 4, 'tok_test8_card3_c31zx8', '7744', 'MAESTRO', 11, 2028, 'BLOCKED', '2026-02-25 09:28:24'),
(10, 4, 'tok_test8_card4_d55mn3', '9901', 'VISA', 2, 2030, 'BLOCKED', '2026-02-25 09:28:24');

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `transfers`
--

CREATE TABLE `transfers` (
  `transfer_id` int(11) NOT NULL,
  `from_account_id` int(11) NOT NULL,
  `to_beneficiary_id` int(11) DEFAULT NULL,
  `to_name` varchar(100) NOT NULL,
  `to_iban` varchar(34) NOT NULL,
  `amount` decimal(12,2) NOT NULL,
  `purpose` varchar(255) DEFAULT NULL,
  `status` enum('pending','executed','failed') DEFAULT 'pending',
  `created_at` timestamp NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;

--
-- Daten für Tabelle `transfers`
--

INSERT INTO `transfers` (`transfer_id`, `from_account_id`, `to_beneficiary_id`, `to_name`, `to_iban`, `amount`, `purpose`, `status`, `created_at`) VALUES
(1, 2, 1, 'Anna Schmidt', 'DE21500105170648481234', 50.00, 'Pizza Abend', 'executed', '2026-02-12 10:01:19'),
(2, 3, 2, 'Max Mustermann', 'DE12500105170648489890', 120.00, 'Schulden zurück', 'pending', '2026-02-12 10:01:19'),
(3, 1, NULL, 'Amazon EU', 'DE89370400440532013000', 89.99, 'Bestellung', 'executed', '2026-02-12 10:01:19'),
(4, 4, NULL, 'Luca Pelka', '9878', 100.00, 'test', 'executed', '2026-03-04 09:17:49'),
(5, 4, NULL, 'Raphael', '97d514841518415', 10000000.00, NULL, 'executed', '2026-03-04 09:18:54'),
(6, 4, NULL, 'anna', 'DE21500105170648481234', 9999.00, NULL, 'executed', '2026-03-04 09:34:44'),
(7, 4, NULL, 'anna', 'DE21500105170648481234', 100.00, NULL, 'executed', '2026-03-04 09:39:48'),
(8, 4, NULL, 'Anna', 'DE21500105170648481234', 12030.00, 'Schutzgeld', 'executed', '2026-03-04 09:40:20'),
(9, 4, NULL, 'Test', 'okdasd', 10.00, NULL, 'executed', '2026-03-04 13:14:33');

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `users`
--

CREATE TABLE `users` (
  `user_id` int(11) NOT NULL,
  `username` varchar(50) NOT NULL,
  `password_hash` varchar(255) NOT NULL,
  `role` enum('USER','ADMIN') DEFAULT 'USER',
  `is_active` tinyint(1) DEFAULT 1,
  `created_at` timestamp NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;

--
-- Daten für Tabelle `users`
--

INSERT INTO `users` (`user_id`, `username`, `password_hash`, `role`, `is_active`, `created_at`) VALUES
(1, 'admin', '$2b$11$gw3MvIQ6HqhbB.vpn2hEjufEHEMce/rMhcSESYOmRumtujiRwnk6y', 'ADMIN', 1, '2026-02-12 10:01:19'),
(2, 'max', '$2b$11$6MDva3C2A2iXkWq3uzTZeeLUSFmxvHgeFCEBez1J2idlCtK0SHy.m', 'USER', 1, '2026-02-12 10:01:19'),
(3, 'anna', '$2b$11$.ZfH8kRPLc2HM.pHocyIAetTwvTPxf8HT1JFirqnxhr7zsej.QWMu', 'USER', 1, '2026-02-12 10:01:19'),
(8, 'test', '$2a$11$Z3N3JlcDAcvOGEkSjNu.YuM428lCYqVOf/SZHtJ.LL3zbmTqsxFie', 'USER', 1, '2026-02-12 10:11:14');

--
-- Indizes der exportierten Tabellen
--

--
-- Indizes für die Tabelle `accounts`
--
ALTER TABLE `accounts`
  ADD PRIMARY KEY (`account_id`),
  ADD UNIQUE KEY `iban` (`iban`),
  ADD KEY `fk_account_user` (`user_id`);

--
-- Indizes für die Tabelle `beneficiaries`
--
ALTER TABLE `beneficiaries`
  ADD PRIMARY KEY (`beneficiary_id`),
  ADD KEY `fk_beneficiary_user` (`user_id`);

--
-- Indizes für die Tabelle `cards`
--
ALTER TABLE `cards`
  ADD PRIMARY KEY (`card_id`),
  ADD UNIQUE KEY `card_token` (`card_token`),
  ADD KEY `fk_cards_account` (`account_id`);

--
-- Indizes für die Tabelle `transfers`
--
ALTER TABLE `transfers`
  ADD PRIMARY KEY (`transfer_id`),
  ADD KEY `fk_transfer_account` (`from_account_id`),
  ADD KEY `fk_transfer_beneficiary` (`to_beneficiary_id`);

--
-- Indizes für die Tabelle `users`
--
ALTER TABLE `users`
  ADD PRIMARY KEY (`user_id`),
  ADD UNIQUE KEY `username` (`username`);

--
-- AUTO_INCREMENT für exportierte Tabellen
--

--
-- AUTO_INCREMENT für Tabelle `accounts`
--
ALTER TABLE `accounts`
  MODIFY `account_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT für Tabelle `beneficiaries`
--
ALTER TABLE `beneficiaries`
  MODIFY `beneficiary_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- AUTO_INCREMENT für Tabelle `cards`
--
ALTER TABLE `cards`
  MODIFY `card_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=11;

--
-- AUTO_INCREMENT für Tabelle `transfers`
--
ALTER TABLE `transfers`
  MODIFY `transfer_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=10;

--
-- AUTO_INCREMENT für Tabelle `users`
--
ALTER TABLE `users`
  MODIFY `user_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=9;

--
-- Constraints der exportierten Tabellen
--

--
-- Constraints der Tabelle `accounts`
--
ALTER TABLE `accounts`
  ADD CONSTRAINT `fk_account_user` FOREIGN KEY (`user_id`) REFERENCES `users` (`user_id`) ON DELETE CASCADE;

--
-- Constraints der Tabelle `beneficiaries`
--
ALTER TABLE `beneficiaries`
  ADD CONSTRAINT `fk_beneficiary_user` FOREIGN KEY (`user_id`) REFERENCES `users` (`user_id`) ON DELETE CASCADE;

--
-- Constraints der Tabelle `cards`
--
ALTER TABLE `cards`
  ADD CONSTRAINT `fk_cards_account` FOREIGN KEY (`account_id`) REFERENCES `accounts` (`account_id`) ON DELETE CASCADE;

--
-- Constraints der Tabelle `transfers`
--
ALTER TABLE `transfers`
  ADD CONSTRAINT `fk_transfer_account` FOREIGN KEY (`from_account_id`) REFERENCES `accounts` (`account_id`) ON DELETE CASCADE,
  ADD CONSTRAINT `fk_transfer_beneficiary` FOREIGN KEY (`to_beneficiary_id`) REFERENCES `beneficiaries` (`beneficiary_id`) ON DELETE SET NULL;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
