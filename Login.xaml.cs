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

            var loginData = GetLoginData(username);

            if (loginData == null)
            {
                MessageBox.Show("Benutzername oder Passwort falsch!", "Login fehlgeschlagen",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            bool ok = BCrypt.Net.BCrypt.Verify(password, loginData.PasswordHash);

            if (!ok)
            {
                MessageBox.Show("Benutzername oder Passwort falsch!", "Login fehlgeschlagen",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string role = (loginData.Role ?? "").Trim().ToLower();

            // Admin -> AdminWindow, User -> MainWindow
            if (role == "admin" || role == "administrator")
            {
                new AdminWindow().Show();
            }
            else
            {
                new MainWindow(loginData.Username).Show();
            }

            Close();
        }

        private class LoginData
        {
            public string Username { get; set; }
            public string PasswordHash { get; set; }
            public string Role { get; set; }
        }

        private LoginData GetLoginData(string username)
        {
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();

                    const string sql = @"
                        SELECT username, password_hash, role
                        FROM users
                        WHERE username = @user AND is_active = 1
                        LIMIT 1;";

                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@user", username);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (!reader.Read())
                                return null;

                            return new LoginData
                            {
                                Username = reader["username"].ToString(),
                                PasswordHash = reader["password_hash"].ToString(),
                                Role = reader["role"].ToString()
                            };
                        }
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
