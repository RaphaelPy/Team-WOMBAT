using MySql.Data.MySqlClient; // brauche ich für die verbindung mit mysql
using System; // grundfunktionen von c#
using System.Data; // brauche ich für datatable
using System.Globalization; // brauche ich um zahlen richtig umzuwandeln
using System.Windows; // für wpf sachen wie messagebox
using System.Windows.Controls; // für page, combobox, datagrid usw.

namespace Banking_app.userpages
{
    public partial class KontoPage : Page // seite für konto und überweisungen
    {
        // speichert die datenbankverbindung
        private readonly string _connectionString;

        // speichert den namen vom aktuell eingeloggten user
        private readonly string _username;

        // speichert die user_id aus der datenbank
        private int _userId = -1;

        // speichert das aktuell ausgewählte konto
        private int _selectedAccountId = -1;

        // konstruktor von der seite
        public KontoPage(string connectionString, string username)
        {
            InitializeComponent(); // lädt das xaml design

            _connectionString = connectionString; // verbindung speichern
            _username = username; // username speichern

            Loaded += KontoPage_Loaded; // wenn die seite geladen wird, wird diese methode aufgerufen
        }

        // wird automatisch ausgeführt wenn die seite geladen ist
        private void KontoPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadUserId(); // user_id zum username aus der datenbank holen

            // wenn keine gültige user_id gefunden wurde
            if (_userId <= 0)
            {
                MessageBox.Show("User nicht gefunden.");
                return;
            }

            LoadAccounts(); // konten vom user laden
        }

        // wenn man auf reload klickt
        private void Reload_Click(object sender, RoutedEventArgs e)
        {
            LoadAccounts(); // konten neu laden

            // wenn schon ein konto ausgewählt ist, dann auch die transfers neu laden
            if (_selectedAccountId > 0)
                LoadTransfers();
        }

        // user_id über username holen
        private void LoadUserId()
        {
            try
            {
                // verbindung zur datenbank erstellen
                using var conn = new MySqlConnection(_connectionString);
                conn.Open(); // verbindung öffnen

                // sql befehl: passende user_id zum username holen
                using var cmd = new MySqlCommand(
                    "SELECT user_id FROM users WHERE username=@u LIMIT 1;", conn);

                // username in die query einsetzen
                cmd.Parameters.AddWithValue("@u", _username);

                // einzelnen wert aus der datenbank holen
                var result = cmd.ExecuteScalar();

                // wenn nichts gefunden wurde -> -1, sonst richtige user_id
                _userId = (result == null) ? -1 : Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                // falls fehler passiert
                MessageBox.Show("Fehler beim Laden der UserId:\n" + ex.Message);
                _userId = -1;
            }
        }

        // accounts in die combobox laden
        private void LoadAccounts()
        {
            try
            {
                // verbindung zur datenbank erstellen
                using var conn = new MySqlConnection(_connectionString);
                conn.Open();

                // alle konten vom user holen
                using var cmd = new MySqlCommand(@"
                    SELECT account_id, iban, balance, currency, status
                    FROM accounts
                    WHERE user_id=@uid
                    ORDER BY account_id;", conn);

                // user_id in die query einsetzen
                cmd.Parameters.AddWithValue("@uid", _userId);

                // datatable erstellen
                var dt = new DataTable();

                // daten aus der datenbank in die datatable laden
                using (var ad = new MySqlDataAdapter(cmd))
                {
                    ad.Fill(dt);
                }

                // datatable an die combobox binden
                CbAccounts.ItemsSource = dt.DefaultView;

                // in der combobox soll die iban angezeigt werden
                CbAccounts.DisplayMemberPath = "iban";

                // als echter wert soll account_id benutzt werden
                CbAccounts.SelectedValuePath = "account_id";

                // wenn konten da sind, erstes konto direkt auswählen
                if (dt.Rows.Count > 0)
                    CbAccounts.SelectedIndex = 0;
                else
                    ClearUi(); // wenn keine konten da sind, oberfläche leeren
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Laden der Konten:\n" + ex.Message);
            }
        }

        // wird ausgeführt wenn in der combobox ein anderes konto ausgewählt wird
        private void CbAccounts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // prüfen ob wirklich ein datensatz ausgewählt wurde
            if (CbAccounts.SelectedItem is not DataRowView row)
                return;

            // account_id vom ausgewählten konto speichern
            _selectedAccountId = Convert.ToInt32(row["account_id"]);

            // daten vom konto auslesen
            decimal balance = Convert.ToDecimal(row["balance"]);
            string currency = row["currency"]?.ToString() ?? "EUR";
            string iban = row["iban"]?.ToString() ?? "";
            string status = row["status"]?.ToString() ?? "";

            // daten im fenster anzeigen
            TxtBalance.Text = $"{balance:0.00} {currency}";
            TxtIban.Text = $"IBAN: {iban}";
            TxtAccStatus.Text = $"Status: {status}";

            // dazugehörige überweisungen laden
            LoadTransfers();
        }

        // überweisungen vom ausgewählten konto laden
        private void LoadTransfers()
        {
            // wenn kein konto ausgewählt ist -> abbrechen
            if (_selectedAccountId <= 0) return;

            try
            {
                using var conn = new MySqlConnection(_connectionString);
                conn.Open();

                // hier werden ausgehende und eingehende überweisungen zusammen geladen
                // out = geld geht von meinem konto weg
                // in = geld kommt auf mein konto
                using var cmd = new MySqlCommand(@"
            SELECT
                t.transfer_id,
                t.created_at,
                'OUT' AS direction,
                t.to_name AS partner,
                t.to_iban AS partner_iban,
                (-t.amount) AS amount_signed,
                t.status
            FROM transfers t
            WHERE t.from_account_id = @aid

            UNION ALL

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
            LEFT JOIN accounts acc_from ON acc_from.account_id = t.from_account_id
            WHERE acc_to.account_id = @aid

            ORDER BY created_at DESC
            LIMIT 50;", conn);

                // account_id einsetzen
                cmd.Parameters.AddWithValue("@aid", _selectedAccountId);

                // datatable erstellen
                var dt = new DataTable();

                // daten laden
                using (var ad = new MySqlDataAdapter(cmd))
                {
                    ad.Fill(dt);
                }

                // daten im datagrid anzeigen
                DgTransfers.ItemsSource = dt.DefaultView;

                // kontodaten nochmal direkt aus der datenbank aktualisieren
                RefreshAccountInfoFromDb();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Laden der Transaktionen:\n" + ex.Message);
            }
        }

        // lädt kontoinfos nochmal direkt aus der datenbank
        private void RefreshAccountInfoFromDb()
        {
            try
            {
                using var conn = new MySqlConnection(_connectionString);
                conn.Open();

                // aktuelle daten von genau diesem konto holen
                using var cmd = new MySqlCommand(@"
                    SELECT iban, balance, currency, status
                    FROM accounts
                    WHERE account_id=@aid
                    LIMIT 1;", conn);

                cmd.Parameters.AddWithValue("@aid", _selectedAccountId);

                // reader um die daten zu lesen
                using var r = cmd.ExecuteReader();

                // wenn nichts gefunden wurde -> abbrechen
                if (!r.Read()) return;

                // daten auslesen
                decimal balance = r.GetDecimal("balance");
                string currency = r.GetString("currency");
                string iban = r.GetString("iban");
                string status = r.GetString("status");

                // daten im fenster anzeigen
                TxtBalance.Text = $"{balance:0.00} {currency}";
                TxtIban.Text = $"IBAN: {iban}";
                TxtAccStatus.Text = $"Status: {status}";
            }
            catch
            {
                // hier wird absichtlich nichts angezeigt
            }
        }

        // wird ausgeführt wenn man auf senden / überweisen klickt
        private void Send_Click(object sender, RoutedEventArgs e)
        {
            // prüfen ob ein konto ausgewählt ist
            if (_selectedAccountId <= 0)
            {
                MessageBox.Show("Bitte zuerst ein Konto auswählen.");
                return;
            }

            // eingaben aus den textboxen holen
            string toName = TbToName.Text.Trim();
            string toIban = TbToIban.Text.Trim();
            string purpose = TbPurpose.Text.Trim();

            // prüfen ob name und iban ausgefüllt sind
            if (string.IsNullOrWhiteSpace(toName) || string.IsNullOrWhiteSpace(toIban))
            {
                MessageBox.Show("Bitte Empfänger Name und IBAN ausfüllen.");
                return;
            }

            // prüfen ob der betrag gültig ist und größer als 0
            if (!TryParseAmount(TbAmount.Text, out decimal amount) || amount <= 0)
            {
                MessageBox.Show("Bitte gültigen Betrag eingeben (z.B. 10,50).");
                return;
            }

            try
            {
                using var conn = new MySqlConnection(_connectionString);
                conn.Open();

                // transaktion starten
                // dadurch wird alles zusammen gespeichert oder bei fehler alles zurückgesetzt
                using var tx = conn.BeginTransaction();

                // empfänger-konto anhand der iban suchen
                int? toAccountId = null;
                using (var cmdFind = new MySqlCommand(
                    "SELECT account_id FROM accounts WHERE iban=@iban LIMIT 1;", conn, tx))
                {
                    cmdFind.Parameters.AddWithValue("@iban", toIban);
                    var r = cmdFind.ExecuteScalar();

                    // wenn ein konto gefunden wurde, account_id speichern
                    if (r != null)
                        toAccountId = Convert.ToInt32(r);
                }

                // aktuellen kontostand vom sender lesen
                // for update sperrt den datensatz kurz für sichere bearbeitung
                decimal currentBalance;
                using (var cmdBal = new MySqlCommand(
                    "SELECT balance FROM accounts WHERE account_id=@aid FOR UPDATE;", conn, tx))
                {
                    cmdBal.Parameters.AddWithValue("@aid", _selectedAccountId);
                    var r = cmdBal.ExecuteScalar();

                    if (r == null) throw new Exception("Sender-Konto nicht gefunden.");

                    currentBalance = Convert.ToDecimal(r);
                }

                // prüfen ob genug geld da ist
                if (currentBalance < amount)
                {
                    tx.Rollback(); // alles zurücksetzen
                    MessageBox.Show("Nicht genügend Guthaben.");
                    return;
                }

                // geld vom sender abbuchen
                using (var cmdUpd = new MySqlCommand(
                    "UPDATE accounts SET balance = balance - @amt WHERE account_id=@aid;", conn, tx))
                {
                    cmdUpd.Parameters.AddWithValue("@amt", amount);
                    cmdUpd.Parameters.AddWithValue("@aid", _selectedAccountId);
                    cmdUpd.ExecuteNonQuery();
                }

                // wenn das empfänger-konto intern existiert, geld dort gutschreiben
                if (toAccountId.HasValue)
                {
                    using var cmdCredit = new MySqlCommand(
                        "UPDATE accounts SET balance = balance + @amt WHERE account_id=@to;", conn, tx);

                    cmdCredit.Parameters.AddWithValue("@amt", amount);
                    cmdCredit.Parameters.AddWithValue("@to", toAccountId.Value);
                    cmdCredit.ExecuteNonQuery();
                }

                // überweisung in der tabelle transfers speichern
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

                    // wenn kein verwendungszweck eingegeben wurde, dann null speichern
                    cmdIns.Parameters.AddWithValue("@purpose",
                        string.IsNullOrWhiteSpace(purpose) ? (object)DBNull.Value : purpose);

                    cmdIns.ExecuteNonQuery();
                }

                tx.Commit(); // alles fest speichern

                // eingabefelder leeren
                TbToName.Clear();
                TbToIban.Clear();
                TbAmount.Clear();
                TbPurpose.Clear();

                LoadTransfers(); // liste neu laden
                MessageBox.Show("Überweisung ausgeführt.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler bei der Überweisung:\n" + ex.Message);
            }
        }

        // versucht den eingegebenen betrag in decimal umzuwandeln
        private bool TryParseAmount(string input, out decimal amount)
        {
            amount = 0m;

            // wenn nichts eingegeben wurde -> false
            if (string.IsNullOrWhiteSpace(input)) return false;

            // leerzeichen entfernen und komma zu punkt machen
            input = input.Trim().Replace(',', '.');

            // versucht den text in eine zahl umzuwandeln
            return decimal.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out amount);
        }

        // setzt die anzeige zurück wenn kein konto da ist
        private void ClearUi()
        {
            _selectedAccountId = -1; // kein konto ausgewählt
            TxtBalance.Text = "—"; // kontostand leeren
            TxtIban.Text = "IBAN: —"; // iban leeren
            TxtAccStatus.Text = "Status: —"; // status leeren
            DgTransfers.ItemsSource = null; // datagrid leeren
        }
    }
}