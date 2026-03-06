using MySql.Data.MySqlClient; // brauche ich um mit einer mysql datenbank zu arbeiten
using System; // grundfunktionen von c#
using System.Windows; // wpf funktionen (fenster, messagebox usw.)
using System.Windows.Controls; // damit ich mit elementen wie page oder textbox arbeiten kann

namespace Banking_app.userpages
{
    public partial class EinstellungenPage : Page // page für die einstellungen im programm
    {
        // speichert die verbindung zur datenbank
        private readonly string _connectionString;

        // speichert den aktuell eingeloggten benutzernamen
        private readonly string _username;

        // konstruktor der page
        public EinstellungenPage(string connectionString, string username)
        {
            InitializeComponent(); // lädt das xaml design der seite

            // speichert die daten aus dem aufruf
            _connectionString = connectionString;
            _username = username;
        }

        // zeigt eine fehlermeldung im textfeld an
        private void ShowError(string msg)
        {
            TxtError.Text = msg; // fehlermeldung in das textfeld schreiben
            TxtError.Visibility = Visibility.Visible; // text sichtbar machen
        }

        // versteckt die fehlermeldung wieder
        private void HideError()
        {
            TxtError.Text = ""; // text löschen
            TxtError.Visibility = Visibility.Collapsed; // textfeld verstecken
        }

        // wird ausgeführt wenn der button geklickt wird
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            HideError(); // zuerst alte fehlermeldung ausblenden

            // passwörter aus den feldern holen
            string oldPw = TxtOldPassword.Password; // aktuelles passwort
            string newPw = TxtNewPassword.Password; // neues passwort
            string repPw = TxtRepeatPassword.Password; // wiederholung vom neuen passwort

            // prüfen ob alle felder ausgefüllt sind
            if (string.IsNullOrWhiteSpace(oldPw) ||
                string.IsNullOrWhiteSpace(newPw) ||
                string.IsNullOrWhiteSpace(repPw))
            {
                ShowError("Bitte alle Felder ausfüllen.");
                return; // methode abbrechen
            }

            // prüfen ob das neue passwort mindestens 4 zeichen hat
            if (newPw.Length < 4)
            {
                ShowError("Neues Passwort muss mindestens 4 Zeichen haben.");
                return;
            }

            // prüfen ob die beiden neuen passwörter gleich sind
            if (newPw != repPw)
            {
                ShowError("Die neuen Passwörter stimmen nicht überein.");
                return;
            }

            // prüfen ob neues passwort gleich dem alten ist
            if (newPw == oldPw)
            {
                ShowError("Neues Passwort darf nicht gleich dem alten sein.");
                return;
            }

            try
            {
                // verbindung zur datenbank erstellen
                using var conn = new MySqlConnection(_connectionString);

                // verbindung öffnen
                conn.Open();

                // variable für den hash aus der datenbank
                string dbHash;

                // sql befehl um das gespeicherte passwort (hash) zu holen
                using (var cmd = new MySqlCommand(
                    "SELECT password_hash FROM users WHERE username=@u LIMIT 1;", conn))
                {
                    // username in die sql query einsetzen
                    cmd.Parameters.AddWithValue("@u", _username);

                    // query ausführen und einen einzelnen wert zurückbekommen
                    var result = cmd.ExecuteScalar();

                    // wenn kein user gefunden wurde
                    if (result == null)
                    {
                        ShowError("User nicht gefunden.");
                        return;
                    }

                    // hash aus der datenbank speichern
                    dbHash = result.ToString();
                }

                // prüfen ob das eingegebene alte passwort zum hash passt
                if (!BCrypt.Net.BCrypt.Verify(oldPw, dbHash))
                {
                    ShowError("Aktuelles Passwort ist falsch.");
                    return;
                }

                // neuen hash aus dem neuen passwort erstellen
                string newHash = BCrypt.Net.BCrypt.HashPassword(newPw);

                // sql befehl um den neuen hash in der datenbank zu speichern
                using (var cmd = new MySqlCommand(
                    "UPDATE users SET password_hash=@h WHERE username=@u LIMIT 1;", conn))
                {
                    // neuen hash einsetzen
                    cmd.Parameters.AddWithValue("@h", newHash);

                    // username einsetzen
                    cmd.Parameters.AddWithValue("@u", _username);

                    // query ausführen (ändert daten)
                    int changed = cmd.ExecuteNonQuery();

                    // wenn nichts geändert wurde -> fehler
                    if (changed != 1)
                    {
                        ShowError("Passwort konnte nicht geändert werden.");
                        return;
                    }
                }

                // erfolgsmeldung anzeigen
                MessageBox.Show("Passwort wurde geändert.", "Erfolg",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                // passwortfelder leeren
                TxtOldPassword.Password = "";
                TxtNewPassword.Password = "";
                TxtRepeatPassword.Password = "";
            }
            catch (Exception ex)
            {
                // falls ein datenbankfehler passiert
                ShowError("DB-Fehler: " + ex.Message);
            }
        }
    }
}