using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace Banking_app
{
    public partial class user : Page
    {
        private string _connectionString;
        private string _username;

        public user()
        {
            InitializeComponent();
        }

        public void SetConnection(string connectionString, string username)
        {
            _connectionString = connectionString;
            _username = username;

            LoadDashboard();
        }

        public void SetWelcomeText(string text)
        {
            txtWelcome.Text = text;
        }

        private void LoadDashboard()
        {
            try
            {
                using var conn = new MySqlConnection(_connectionString);
                conn.Open();

                // Gesamtguthaben laden
                using (var cmd = new MySqlCommand(@"
                    SELECT SUM(balance)
                    FROM accounts a
                    JOIN users u ON u.user_id = a.user_id
                    WHERE u.username=@u;", conn))
                {
                    cmd.Parameters.AddWithValue("@u", _username);

                    var result = cmd.ExecuteScalar();
                    decimal balance = result == DBNull.Value ? 0 : Convert.ToDecimal(result);

                    txtBalance.Text = $"{balance:0.00} €";
                }

                // letzte Transaktionen
                using (var cmd = new MySqlCommand(@"
                    SELECT created_at, to_name, amount, status
                    FROM transfers t
                    JOIN accounts a ON a.account_id = t.from_account_id
                    JOIN users u ON u.user_id = a.user_id
                    WHERE u.username=@u
                    ORDER BY created_at DESC
                    LIMIT 5;", conn))
                {
                    cmd.Parameters.AddWithValue("@u", _username);

                    var dt = new DataTable();
                    new MySqlDataAdapter(cmd).Fill(dt);

                    dgRecentTransfers.ItemsSource = dt.DefaultView;
                }
            }
            catch
            {
            }
        }
        private void Gotokonto(object sender, RoutedEventArgs e)
        {
            var main = Window.GetWindow(this) as MainWindow;
            if (main == null) return; // passiert nur im Designer

            main.MainFrame.Navigate(new Banking_app.userpages.KontoPage(_connectionString, _username));
        }

        private void gototransaktionen(object sender, RoutedEventArgs e)
        {
            var main = Window.GetWindow(this) as MainWindow;
            if (main == null) return;

            main.MainFrame.Navigate(new Banking_app.userpages.TransaktionenPage(_connectionString, _username));
        }

        private void gotokarten(object sender, RoutedEventArgs e)
        {
            var main = Window.GetWindow(this) as MainWindow;
            if (main == null) return;

            main.MainFrame.Navigate(new Banking_app.userpages.KartenPage(_connectionString, _username));
        }
    }
    }
