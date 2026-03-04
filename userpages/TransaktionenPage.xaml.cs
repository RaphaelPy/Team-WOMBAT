using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace Banking_app.userpages
{
    public partial class TransaktionenPage : Page
    {
        
        private readonly string _connectionString;
        private readonly string _username;

        
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

        
        private void LoadTransfers()
        {
            try
            {
                using var conn = new MySqlConnection(_connectionString);
                conn.Open();

                
                string sql = @"
            -- OUT: von mir weg (minus)
            SELECT
                t.transfer_id,
                t.created_at,
                'OUT' AS direction,
                t.to_name AS partner,
                t.to_iban AS partner_iban,
                (-t.amount) AS amount_signed,
                t.status
            FROM transfers t
            JOIN accounts a ON a.account_id = t.from_account_id
            JOIN users u ON u.user_id = a.user_id
            WHERE u.username = @u

            UNION ALL

            -- IN: zu mir hin (plus) - Empfänger IBAN ist eine meiner IBANs
            SELECT
                t.transfer_id,
                t.created_at,
                'IN' AS direction,
                CONCAT('Von Konto ', t.from_account_id) AS partner,
                acc_from.iban AS partner_iban,
                (t.amount) AS amount_signed,
                t.status
            FROM transfers t
            JOIN accounts acc_to ON acc_to.iban = t.to_iban
            JOIN users u_to ON u_to.user_id = acc_to.user_id
            LEFT JOIN accounts acc_from ON acc_from.account_id = t.from_account_id
            WHERE u_to.username = @u

            ORDER BY created_at DESC;";

                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@u", _username);

                var dt = new DataTable();
                using (var ad = new MySqlDataAdapter(cmd))
                {
                    ad.Fill(dt);
                }

                dgTransfers.ItemsSource = dt.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Laden:\n" + ex.Message);
            }
        }
    }
}