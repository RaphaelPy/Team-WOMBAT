using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace Banking_app.userpages
{
    public partial class KartenPage : Page
    {
        
        private readonly string _connectionString;
        private readonly string _username;

        public KartenPage(string connectionString, string username)
        {
            InitializeComponent();

            _connectionString = connectionString;
            _username = username;

            Loaded += KartenPage_Loaded;
        }

        private void KartenPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadCards();
        }

        
        private void Reload_Click(object sender, RoutedEventArgs e)
        {
            LoadCards();
        }

  
        private void LoadCards()
        {
            try
            {
                using var conn = new MySqlConnection(_connectionString);
                conn.Open();

                // Ich hole alle Karten, die über accounts zu meinem username gehören
                string sql = @"
                    SELECT 
                        c.card_id,
                        c.account_id,
                        c.last4,
                        c.brand,
                        c.exp_month,
                        c.exp_year,
                        c.status,
                        c.created_at
                    FROM cards c
                    JOIN accounts a ON a.account_id = c.account_id
                    JOIN users u ON u.user_id = a.user_id
                    WHERE u.username = @u
                    ORDER BY c.account_id, c.card_id;";

                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@u", _username);

                var dt = new DataTable();
                new MySqlDataAdapter(cmd).Fill(dt);

                
                dgCards.ItemsSource = dt.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Laden:\n" + ex.Message);
            }
        }

        private (int CardId, string Status)? GetSelectedCard()
        {
            if (dgCards.SelectedItem is not DataRowView row)
                return null;

            int cardId = Convert.ToInt32(row["card_id"]);
            string status = row["status"]?.ToString() ?? "ACTIVE";

            return (cardId, status);
        }


        private bool SetCardStatus(int cardId, string newStatus)
        {
            try
            {
                using var conn = new MySqlConnection(_connectionString);
                conn.Open();

                string sql = "UPDATE cards SET status=@s WHERE card_id=@id LIMIT 1;";

                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@s", newStatus);
                cmd.Parameters.AddWithValue("@id", cardId);

                return cmd.ExecuteNonQuery() == 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Update:\n" + ex.Message);
                return false;
            }
        }

        // Button: Sperren / Entsperren
        private void ToggleBlock_Click(object sender, RoutedEventArgs e)
        {
            
            var selected = GetSelectedCard();
            if (selected == null)
            {
                MessageBox.Show("Bitte zuerst eine Karte auswählen.");
                return;
            }

          
            int cardId = selected.Value.CardId;
            string oldStatus = selected.Value.Status;

            string newStatus = oldStatus.Equals("BLOCKED", StringComparison.OrdinalIgnoreCase)
                ? "ACTIVE"
                : "BLOCKED";

            
            if (SetCardStatus(cardId, newStatus))
            {
                LoadCards();
            }
        }
    }
}