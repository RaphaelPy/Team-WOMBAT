using System;
using System.Windows;
using MySql.Data.MySqlClient;

namespace Banking_app
{
    public partial class Login : Window
    {
        private readonly string _connectionString =
     "server=mysql.pb.bib.de;uid=pbt3h24akr;pwd=zJpyj6GPvtK6;database=pbt3h24akr_Wombank";

        public Login()
        {
            InitializeComponent();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUser.Text.Trim();
            string password = txtPassword.Password;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Bitte Benutzername und Passwort eingeben.");
                return;
            }

            string storedHash = GetPasswordHash(username);

            if (storedHash == null)
            {
                MessageBox.Show("Benutzername oder Passwort falsch!", "Login fehlgeschlagen",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            
            bool ok = BCrypt.Net.BCrypt.Verify(password, storedHash);

            if (ok)
            {
                MainWindow main = new MainWindow();
                main.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Benutzername oder Passwort falsch!", "Login fehlgeschlagen",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string GetPasswordHash(string username)
        {
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();

                    const string sql = @"SELECT password_hash 
                                         FROM users 
                                         WHERE username = @user AND is_active = 1
                                         LIMIT 1;";

                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@user", username);

                        object result = cmd.ExecuteScalar();
                        return result == null ? null : result.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("DB-Fehler: " + ex.Message, "Fehler",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }
    }
}
