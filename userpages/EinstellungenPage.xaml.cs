using MySql.Data.MySqlClient;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Banking_app.userpages
{
    public partial class EinstellungenPage : Page
    {
        private readonly string _connectionString;
        private readonly string _username;

        public EinstellungenPage(string connectionString, string username)
        {
            InitializeComponent();
            _connectionString = connectionString;
            _username = username;
        }

        private void ShowError(string msg)
        {
            TxtError.Text = msg;
            TxtError.Visibility = Visibility.Visible;
        }

        private void HideError()
        {
            TxtError.Text = "";
            TxtError.Visibility = Visibility.Collapsed;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            HideError();

            string oldPw = TxtOldPassword.Password;
            string newPw = TxtNewPassword.Password;
            string repPw = TxtRepeatPassword.Password;

            if (string.IsNullOrWhiteSpace(oldPw) ||
                string.IsNullOrWhiteSpace(newPw) ||
                string.IsNullOrWhiteSpace(repPw))
            {
                ShowError("Bitte alle Felder ausfüllen.");
                return;
            }

            if (newPw.Length < 4)
            {
                ShowError("Neues Passwort muss mindestens 4 Zeichen haben.");
                return;
            }

            if (newPw != repPw)
            {
                ShowError("Die neuen Passwörter stimmen nicht überein.");
                return;
            }

            if (newPw == oldPw)
            {
                ShowError("Neues Passwort darf nicht gleich dem alten sein.");
                return;
            }

            try
            {
                using var conn = new MySqlConnection(_connectionString);
                conn.Open();

                // 1) Hash aus DB holen
                string dbHash;
                using (var cmd = new MySqlCommand(
                    "SELECT password_hash FROM users WHERE username=@u LIMIT 1;", conn))
                {
                    cmd.Parameters.AddWithValue("@u", _username);
                    var result = cmd.ExecuteScalar();

                    if (result == null)
                    {
                        ShowError("User nicht gefunden.");
                        return;
                    }

                    dbHash = result.ToString();
                }

                // 2) Altes Passwort prüfen
                if (!BCrypt.Net.BCrypt.Verify(oldPw, dbHash))
                {
                    ShowError("Aktuelles Passwort ist falsch.");
                    return;
                }

                // 3) Neuen Hash erstellen + speichern
                string newHash = BCrypt.Net.BCrypt.HashPassword(newPw);

                using (var cmd = new MySqlCommand(
                    "UPDATE users SET password_hash=@h WHERE username=@u LIMIT 1;", conn))
                {
                    cmd.Parameters.AddWithValue("@h", newHash);
                    cmd.Parameters.AddWithValue("@u", _username);

                    int changed = cmd.ExecuteNonQuery();
                    if (changed != 1)
                    {
                        ShowError("Passwort konnte nicht geändert werden.");
                        return;
                    }
                }

                MessageBox.Show("Passwort wurde geändert.", "Erfolg",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                TxtOldPassword.Password = "";
                TxtNewPassword.Password = "";
                TxtRepeatPassword.Password = "";
            }
            catch (Exception ex)
            {
                ShowError("DB-Fehler: " + ex.Message);
            }
        }
    }
}