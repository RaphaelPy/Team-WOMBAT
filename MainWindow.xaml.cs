using MySql.Data.MySqlClient;
using System;
using System.Windows;

namespace Banking_app
{
    public partial class MainWindow : Window
    {
        // DB bleibt im Window
        private readonly string _connectionString =
            "server=mysql.pb.bib.de;uid=pbt3h24akr;pwd=zJpyj6GPvtK6;database=pbt3h24akr_Wombank";

        private readonly string _username; 
        private user _userPage;           

        // Username wird übergeben
        public MainWindow(string username)
        {
            InitializeComponent();
            _username = username;
        }

        // Page laden
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _userPage = new user();
            MainFrame.Navigate(_userPage);

            //  Window pusht Daten in die Page
            _userPage.SetWelcomeText($"Willkommen, {_username}!");

            // Wenn du später DB-Userdaten laden willst: LoadUserData(); (für Raphael)
        }

        private void LoadUserData()
        {
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();

                    string sql = @"SELECT username FROM users WHERE username=@u LIMIT 1;";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@u", _username);
                        var result = cmd.ExecuteScalar()?.ToString() ?? _username;

                        _userPage?.SetWelcomeText($"Willkommen, {result}!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("DB Fehler:\n" + ex.Message);
            }
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            new Login().Show();
            Close();
        }
    }
}
