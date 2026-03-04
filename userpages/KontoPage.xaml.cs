using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace Banking_app.userpages
{
    public partial class KontoPage : Page
    {
        private readonly string _connectionString;
        private readonly string _username;

        private int _userId = -1;
        private int _selectedAccountId = -1;

        public KontoPage(string connectionString, string username)
        {
            InitializeComponent();

            _connectionString = connectionString;
            _username = username;

            Loaded += KontoPage_Loaded;
        }

        private void KontoPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadUserId();
            if (_userId <= 0)
            {
                MessageBox.Show("User nicht gefunden.");
                return;
            }

            LoadAccounts();
        }

        private void Reload_Click(object sender, RoutedEventArgs e)
        {
            LoadAccounts();
            if (_selectedAccountId > 0)
                LoadTransfers();
        }

        // ===== 1) user_id über username holen =====
        private void LoadUserId()
        {
            try
            {
                using var conn = new MySqlConnection(_connectionString);
                conn.Open();

                using var cmd = new MySqlCommand(
                    "SELECT user_id FROM users WHERE username=@u LIMIT 1;", conn);

                cmd.Parameters.AddWithValue("@u", _username);

                var result = cmd.ExecuteScalar();
                _userId = (result == null) ? -1 : Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Laden der UserId:\n" + ex.Message);
                _userId = -1;
            }
        }

        // ===== 2) Accounts in ComboBox laden =====
        private void LoadAccounts()
        {
            try
            {
                using var conn = new MySqlConnection(_connectionString);
                conn.Open();

                using var cmd = new MySqlCommand(@"
                    SELECT account_id, iban, balance, currency, status
                    FROM accounts
                    WHERE user_id=@uid
                    ORDER BY account_id;", conn);

                cmd.Parameters.AddWithValue("@uid", _userId);

                var dt = new DataTable();
                using (var ad = new MySqlDataAdapter(cmd))
                {
                    ad.Fill(dt);
                }

                CbAccounts.ItemsSource = dt.DefaultView;
                CbAccounts.DisplayMemberPath = "iban";
                CbAccounts.SelectedValuePath = "account_id";

                if (dt.Rows.Count > 0)
                    CbAccounts.SelectedIndex = 0;
                else
                    ClearUi();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Laden der Konten:\n" + ex.Message);
            }
        }

        // ===== 3) Konto auswählen -> Kontostand + Transfers =====
        private void CbAccounts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CbAccounts.SelectedItem is not DataRowView row)
                return;

            _selectedAccountId = Convert.ToInt32(row["account_id"]);

            decimal balance = Convert.ToDecimal(row["balance"]);
            string currency = row["currency"]?.ToString() ?? "EUR";
            string iban = row["iban"]?.ToString() ?? "";
            string status = row["status"]?.ToString() ?? "";

            TxtBalance.Text = $"{balance:0.00} {currency}";
            TxtIban.Text = $"IBAN: {iban}";
            TxtAccStatus.Text = $"Status: {status}";

            LoadTransfers();
        }

        // ===== 4) Transfers laden =====
        private void LoadTransfers()
        {
            if (_selectedAccountId <= 0) return;

            try
            {
                using var conn = new MySqlConnection(_connectionString);
                conn.Open();

                using var cmd = new MySqlCommand(@"
                    SELECT transfer_id, to_name, to_iban, amount, purpose, status, created_at
                    FROM transfers
                    WHERE from_account_id=@aid
                    ORDER BY created_at DESC
                    LIMIT 50;", conn);

                cmd.Parameters.AddWithValue("@aid", _selectedAccountId);

                var dt = new DataTable();
                using (var ad = new MySqlDataAdapter(cmd))
                {
                    ad.Fill(dt);
                }

                DgTransfers.ItemsSource = dt.DefaultView;

                // Kontostand nochmal frisch aus DB holen (falls er sich geändert hat)
                RefreshAccountInfoFromDb();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Laden der Transaktionen:\n" + ex.Message);
            }
        }

        private void RefreshAccountInfoFromDb()
        {
            try
            {
                using var conn = new MySqlConnection(_connectionString);
                conn.Open();

                using var cmd = new MySqlCommand(@"
                    SELECT iban, balance, currency, status
                    FROM accounts
                    WHERE account_id=@aid
                    LIMIT 1;", conn);

                cmd.Parameters.AddWithValue("@aid", _selectedAccountId);

                using var r = cmd.ExecuteReader();
                if (!r.Read()) return;

                decimal balance = r.GetDecimal("balance");
                string currency = r.GetString("currency");
                string iban = r.GetString("iban");
                string status = r.GetString("status");

                TxtBalance.Text = $"{balance:0.00} {currency}";
                TxtIban.Text = $"IBAN: {iban}";
                TxtAccStatus.Text = $"Status: {status}";
            }
            catch
            {
               
            }
        }

        // ===== 5) Überweisung ausführen =====
        private void Send_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedAccountId <= 0)
            {
                MessageBox.Show("Bitte zuerst ein Konto auswählen.");
                return;
            }

            string toName = TbToName.Text.Trim();
            string toIban = TbToIban.Text.Trim();
            string purpose = TbPurpose.Text.Trim();

            if (string.IsNullOrWhiteSpace(toName) || string.IsNullOrWhiteSpace(toIban))
            {
                MessageBox.Show("Bitte Empfänger Name und IBAN ausfüllen.");
                return;
            }

            if (!TryParseAmount(TbAmount.Text, out decimal amount) || amount <= 0)
            {
                MessageBox.Show("Bitte gültigen Betrag eingeben (z.B. 10,50).");
                return;
            }

            try
            {
                using var conn = new MySqlConnection(_connectionString);
                conn.Open();
                using var tx = conn.BeginTransaction();

                // Balance lesen (gesperrt)
                decimal currentBalance;
                using (var cmdBal = new MySqlCommand(
                    "SELECT balance FROM accounts WHERE account_id=@aid FOR UPDATE;", conn, tx))
                {
                    cmdBal.Parameters.AddWithValue("@aid", _selectedAccountId);
                    var r = cmdBal.ExecuteScalar();
                    if (r == null) throw new Exception("Konto nicht gefunden.");
                    currentBalance = Convert.ToDecimal(r);
                }

                if (currentBalance < amount)
                {
                    tx.Rollback();
                    MessageBox.Show("Nicht genügend Guthaben.");
                    return;
                }

                // Balance abziehen
                using (var cmdUpd = new MySqlCommand(
                    "UPDATE accounts SET balance = balance - @amt WHERE account_id=@aid;", conn, tx))
                {
                    cmdUpd.Parameters.AddWithValue("@amt", amount);
                    cmdUpd.Parameters.AddWithValue("@aid", _selectedAccountId);
                    cmdUpd.ExecuteNonQuery();
                }

                // Transfer speichern
                using (var cmdIns = new MySqlCommand(@"
                    INSERT INTO transfers
                    (from_account_id, to_beneficiary_id, to_name, to_iban, amount, purpose, status)
                    VALUES
                    (@from, NULL, @name, @iban, @amt, @purpose, 'executed');", conn, tx))
                {
                    cmdIns.Parameters.AddWithValue("@from", _selectedAccountId);
                    cmdIns.Parameters.AddWithValue("@name", toName);
                    cmdIns.Parameters.AddWithValue("@iban", toIban);
                    cmdIns.Parameters.AddWithValue("@amt", amount);
                    cmdIns.Parameters.AddWithValue("@purpose",
                        string.IsNullOrWhiteSpace(purpose) ? (object)DBNull.Value : purpose);

                    cmdIns.ExecuteNonQuery();
                }

                tx.Commit();

                TbToName.Clear();
                TbToIban.Clear();
                TbAmount.Clear();
                TbPurpose.Clear();

                LoadTransfers();
                MessageBox.Show("Überweisung ausgeführt.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler bei der Überweisung:\n" + ex.Message);
            }
        }

        private bool TryParseAmount(string input, out decimal amount)
        {
            amount = 0m;
            if (string.IsNullOrWhiteSpace(input)) return false;

            input = input.Trim().Replace(',', '.');
            return decimal.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out amount);
        }

        private void ClearUi()
        {
            _selectedAccountId = -1;
            TxtBalance.Text = "—";
            TxtIban.Text = "IBAN: —";
            TxtAccStatus.Text = "Status: —";
            DgTransfers.ItemsSource = null;
        }
    }
}