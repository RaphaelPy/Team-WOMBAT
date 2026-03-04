using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace Banking_app.userpages
{
    public partial class TransaktionenPage : Page
    {
        // Ich merke mir DB-Verbindung und den eingeloggten User
        private readonly string _connectionString;
        private readonly string _username;

        // Ich speichere die geladenen Daten, damit ich filtern kann
        private DataTable _data;

        public TransaktionenPage(string connectionString, string username)
        {
            InitializeComponent();
            _connectionString = connectionString;
            _username = username;

            Loaded += (_, __) => LoadTransfers();
        }

        // Button: Neu laden
        private void Reload_Click(object sender, RoutedEventArgs e)
        {
            LoadTransfers();
        }

        // Button: Alle anzeigen
        private void ShowAll_Click(object sender, RoutedEventArgs e)
        {
            if (_data == null) return;
            dgTransfers.ItemsSource = _data.DefaultView;
        }

        // Button: Nur fehlgeschlagen
        private void ShowOnlyFailed_Click(object sender, RoutedEventArgs e)
        {
            if (_data == null) return;

            var view = _data.DefaultView;
            view.RowFilter = "status = 'failed'";
            dgTransfers.ItemsSource = view;
        }

        // Ich lade alle Transfers, die von meinen Accounts ausgehen
        private void LoadTransfers()
        {
            try
            {
                using var conn = new MySqlConnection(_connectionString);
                conn.Open();

                string sql = @"
                    SELECT
                        t.transfer_id,
                        t.from_account_id,
                        t.to_beneficiary_id,
                        t.to_name,
                        t.to_iban,
                        t.amount,
                        t.purpose,
                        t.status,
                        t.created_at
                    FROM transfers t
                    JOIN accounts a ON a.account_id = t.from_account_id
                    JOIN users u ON u.user_id = a.user_id
                    WHERE u.username = @u
                    ORDER BY t.created_at DESC;";

                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@u", _username);

                _data = new DataTable();
                new MySqlDataAdapter(cmd).Fill(_data);

                // Filter zurücksetzen
                _data.DefaultView.RowFilter = "";

                dgTransfers.ItemsSource = _data.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Laden:\n" + ex.Message);
            }
        }
    }
}