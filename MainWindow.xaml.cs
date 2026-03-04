using Banking_app.userpages;
using MySql.Data.MySqlClient;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;


namespace Banking_app
{
    public partial class MainWindow : Window
    {
        private readonly string _connectionString =
            "server=mysql.pb.bib.de;uid=pbt3h24akr;pwd=zJpyj6GPvtK6;database=pbt3h24akr_Wombank";

        private readonly string _username;

        private user _userPage;

        private DispatcherTimer _kickTimer;
        private bool _isLoggingOut = false;

        public MainWindow(string username)
        {
            InitializeComponent();
            _username = username;

            
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Sicherheit: Falls irgendwas schief ist, nicht crashen
            if (MainFrame == null)
            {
                MessageBox.Show("MainFrame wurde nicht geladen!");
                return;
            }

            NavigateToStart();
            StartLiveKickCheck();
        }

        // =========================
        // ===== NAVIGATION ========
        // =========================

        private void NavigateToStart()
        {
            _userPage = new user();
            MainFrame.Navigate(_userPage);
            _userPage.SetWelcomeText($"Willkommen, {_username}!");
        }

        private void MenuList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MainFrame == null) return;
            if (_isLoggingOut) return;

            if (MenuList.SelectedItem is not ListBoxItem item)
                return;

            string choice = item.Content?.ToString() ?? "";

            switch (choice)
            {
                case "Start":
                    NavigateToStart();
                    break;

                case "Konto":
                    MainFrame.Navigate(new Banking_app.userpages.KontoPage(_connectionString, _username));
                    break;

                case "Transaktionen":
                    MainFrame.Navigate(new Banking_app.userpages.TransaktionenPage(_connectionString, _username));
                    break;

                case "Karten":
                    MainFrame.Navigate(new KartenPage(_connectionString, _username));
                    break;

                case "Einstellungen":
                    MainFrame.Navigate(new EinstellungenPage());
                    break;
            }
        }

        // =========================
        // ===== LIVE KICK =========
        // =========================

        private void StartLiveKickCheck()
        {
            _kickTimer = new DispatcherTimer();
            _kickTimer.Interval = TimeSpan.FromSeconds(3);
            _kickTimer.Tick += KickTimer_Tick;
            _kickTimer.Start();
        }

        private void KickTimer_Tick(object sender, EventArgs e)
        {
            if (_isLoggingOut) return;

            if (!IsUserStillActive())
            {
                ForceLogoutBecauseBlocked();
            }
        }

        private bool IsUserStillActive()
        {
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();

                    const string sql =
                        @"SELECT is_active FROM users WHERE username=@u LIMIT 1;";

                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@u", _username);
                        var result = cmd.ExecuteScalar();

                        if (result == null)
                            return false;

                        return Convert.ToInt32(result) == 1;
                    }
                }
            }
            catch
            {
                // Wenn DB kurz nicht erreichbar ist -> nicht sofort rauswerfen
                return true;
            }
        }

        private void ForceLogoutBecauseBlocked()
        {
            _isLoggingOut = true;
            _kickTimer?.Stop();

            MessageBox.Show("Dein Account wurde gesperrt. Du wirst abgemeldet.");

            new Login().Show();
            Close();
        }

        // =========================
        // ===== LOGOUT ============
        // =========================

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            _isLoggingOut = true;
            _kickTimer?.Stop();

            new Login().Show();
            Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            _kickTimer?.Stop();
            base.OnClosed(e);
        }
    }
}