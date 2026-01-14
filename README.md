# Banking App – Projektplanung (KW03)

## Projektübersicht
Dieses Projekt ist eine einfache **Banking-App mit MySQL-Datenbankanbindung**, die über **phpMyAdmin** verwaltet wird.  
Ziel ist die Umsetzung grundlegender Banking-Funktionen unter Berücksichtigung von Sicherheit, Datenvalidierung und sauberer Architektur.

Die Anwendung ist eine **Multiview-Anwendung** und unterstützt **mindestens zwei Benutzerrollen**.

---

## Projektziel
Entwicklung einer Banking-App, mit der:
- Benutzer ihre Kontodaten einsehen können
- Empfänger verwaltet werden können
- Überweisungen erstellt und angezeigt werden
- Administratoren Benutzer verwalten können

---

## Muss- und Kann-Funktionen

### Muss-Funktionen
- Login mit Benutzername und Passwort
- Zwei Rollen:
  - USER
  - ADMIN
- Rollenbasierte Zugriffskontrolle
- Kontoübersicht (IBAN, Kontostand, Währung)
- Empfänger verwalten (vollständiges CRUD)
- Überweisungen erstellen und anzeigen
- MySQL-Datenbankanbindung (phpMyAdmin)
- Fehlerbehandlung beim Datenbankzugriff
- Sinnvolle Validierung aller Eingaben
- Schutz vor SQL-Injections (Prepared Statements)
- Datenmodell mit mindestens 4 Tabellen
- Multiview-Anwendung mit mindestens 4 Views

### Kann-Funktionen (optional)
- Suche und Filter für Überweisungen
- Export von Überweisungen (CSV)
- Kategorien für Überweisungen
- Passwort ändern
- Soft-Delete für Empfänger

---

## Multiview-Anwendung (Views)

### View 1: Login
- Eingabe von Benutzername und Passwort
- Prüfung der Zugangsdaten
- Fehlermeldungen bei falschen Daten oder gesperrten Benutzern

---

### View 2: Dashboard / Kontoübersicht
- Anzeige:
  - IBAN
  - Kontostand
  - Währung
  - Kontostatus
- Navigation zu:
  - Empfänger
  - Überweisungen
  - Logout

---

### View 3: Empfänger verwalten (CRUD)
- Anzeige aller Empfänger
- Funktionen:
  - Empfänger anlegen (Create)
  - Empfänger anzeigen (Read)
  - Empfänger bearbeiten (Update)
  - Empfänger löschen (Delete)

> Diese View dient als vollständiger CRUD-Nachweis.

---

### View 4: Überweisungen
- Formular zum Erstellen einer Überweisung
- Anzeige der Überweisungs-Historie
- Anzeige des Status (pending, executed, failed)

---

### Admin View (optional / integriert)
- Benutzerliste
- Benutzer aktivieren / deaktivieren
- Rollen anzeigen

---

## Datenmodell (MySQL / phpMyAdmin)

### Tabelle: users
- user_id (INT, PK, AUTO_INCREMENT)
- username (VARCHAR, UNIQUE)
- password_hash (VARCHAR)
- role (ENUM: USER, ADMIN)
- is_active (BOOLEAN)
- created_at (TIMESTAMP)

---

### Tabelle: accounts
- account_id (INT, PK, AUTO_INCREMENT)
- user_id (INT, FK)
- iban (VARCHAR, UNIQUE)
- balance (DECIMAL)
- currency (CHAR)
- status (ENUM: OPEN, FROZEN, CLOSED)
- created_at (TIMESTAMP)

---

### Tabelle: beneficiaries
- beneficiary_id (INT, PK, AUTO_INCREMENT)
- user_id (INT, FK)
- name (VARCHAR)
- iban (VARCHAR)
- bic (VARCHAR, optional)
- nickname (VARCHAR, optional)
- created_at (TIMESTAMP)

---

### Tabelle: transfers
- transfer_id (INT, PK, AUTO_INCREMENT)
- from_account_id (INT, FK)
- to_beneficiary_id (INT, FK, optional)
- to_name (VARCHAR)
- to_iban (VARCHAR)
- amount (DECIMAL)
- purpose (VARCHAR)
- status (ENUM: pending, executed, failed)
- created_at (TIMESTAMP)

---

## CRUD-Nachweis
- Create: Empfänger anlegen, Überweisung erstellen
- Read: Kontoübersicht, Empfängerliste, Überweisungsübersicht
- Update: Empfänger bearbeiten, Benutzer aktivieren/deaktivieren
- Delete: Empfänger löschen

---

## Datenbankanbindung
- MySQL-Datenbank
- Verwaltung über phpMyAdmin
- Zugriff aus der Anwendung über z. B.:
  - PDO (PHP)
  - MySQLi (Prepared Statements)

**Wichtig:**  
Alle SQL-Befehle werden ausschließlich mit **Prepared Statements** ausgeführt.

---

## Fehlerbehandlung beim DB-Zugriff
- Jeder Datenbankzugriff ist in Try/Catch-Blöcken gekapselt
- Typische Fehler:
  - Duplicate Entry (z. B. Username oder IBAN existiert bereits)
  - Foreign-Key-Fehler
  - Verbindungsprobleme
- Benutzer erhalten verständliche Fehlermeldungen
- Technische Details werden intern geloggt

---

## Validierung

### Login
- Benutzername Pflichtfeld (min. 3 Zeichen)
- Passwort Pflichtfeld (min. 6 Zeichen)

### Empfänger
- Name Pflichtfeld (2–100 Zeichen)
- IBAN Pflichtfeld (15–34 Zeichen, alphanumerisch)
- BIC optional (8 oder 11 Zeichen)

### Überweisung
- Betrag > 0
- Maximalbetrag (z. B. 10.000)
- Verwendungszweck max. 140 Zeichen
- Empfänger-IBAN Pflicht

---

## Sicherheit
- Schutz vor SQL-Injections durch Prepared Statements
- Passwörter werden gehasht gespeichert (z. B. password_hash in PHP)
- Rollenbasierte Zugriffskontrolle
- Kein direkter SQL-Zugriff aus Views

---

## Zeitplanung (40 Stunden)

### Planung & Design – 6h
- Funktionsdefinition
- View-Skizzen
- Datenbankmodell

### Datenbank & Backend – 12h
- MySQL-Tabellen (phpMyAdmin)
- CRUD-Funktionen
- Login & Rollenlogik

### Frontend / Views – 14h
- Login View
- Dashboard
- Empfänger CRUD
- Überweisungen

### Tests & Dokumentation – 8h
- Validierung
- Fehlerbehandlung
- Tests
- README & Screenshots

---

## Abgabe
- README.md
- Quellcode
- SQL-Datei (CREATE TABLE)
- Screenshots der Anwendung
