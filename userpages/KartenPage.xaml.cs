using MySql.Data.MySqlClient; // brauche ich für die verbindung mit mysql
using System; // grundfunktionen von c#
using System.Data; // brauche ich für datatable
using System.Windows; // für wpf sachen wie messagebox
using System.Windows.Controls; // für page, buttons, datagrid usw.

namespace Banking_app.userpages
{
    public partial class KartenPage : Page // seite für die kartenverwaltung
    {
        // speichert die datenbankverbindung
        private readonly string _connectionString;

        // speichert den benutzernamen vom aktuell eingeloggten user
        private readonly string _username;

        // konstruktor von der seite
        public KartenPage(string connectionString, string username)
        {
            InitializeComponent(); // lädt das xaml design

            _connectionString = connectionString; // verbindung speichern
            _username = username; // username speichern

            Loaded += KartenPage_Loaded; // wenn die seite geladen wird, wird die methode unten ausgeführt
        }

        // wird automatisch aufgerufen, wenn die seite fertig geladen ist
        private void KartenPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadCards(); // karten aus der datenbank laden
        }

        // wenn man auf den reload button klickt
        private void Reload_Click(object sender, RoutedEventArgs e)
        {
            LoadCards(); // kartenliste neu laden
        }

        // lädt alle karten vom aktuellen benutzer aus der datenbank
        private void LoadCards()
        {
            try
            {
                // verbindung zur datenbank erstellen
                using var conn = new MySqlConnection(_connectionString);
                conn.Open(); // verbindung öffnen

                // ich hole alle karten, die über accounts zu meinem username gehören
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

                // sql befehl erstellen
                using var cmd = new MySqlCommand(sql, conn);

                // username an die query übergeben
                cmd.Parameters.AddWithValue("@u", _username);

                // datatable anlegen, damit die daten gespeichert werden können
                var dt = new DataTable();

                // daten aus der datenbank in die datatable laden
                new MySqlDataAdapter(cmd).Fill(dt);

                // datatable im datagrid anzeigen
                dgCards.ItemsSource = dt.DefaultView;
            }
            catch (Exception ex)
            {
                // falls ein fehler beim laden passiert
                MessageBox.Show("Fehler beim Laden:\n" + ex.Message);
            }
        }

        // holt die aktuell ausgewählte karte aus dem datagrid
        private (int CardId, string Status)? GetSelectedCard()
        {
            // prüfen ob überhaupt eine zeile ausgewählt wurde
            if (dgCards.SelectedItem is not DataRowView row)
                return null;

            // card_id aus der ausgewählten zeile holen
            int cardId = Convert.ToInt32(row["card_id"]);

            // status aus der ausgewählten zeile holen
            string status = row["status"]?.ToString() ?? "ACTIVE";

            // card_id und status zurückgeben
            return (cardId, status);
        }

        // ändert den status einer karte in der datenbank
        private bool SetCardStatus(int cardId, string newStatus)
        {
            try
            {
                // verbindung zur datenbank erstellen
                using var conn = new MySqlConnection(_connectionString);
                conn.Open(); // verbindung öffnen

                // sql befehl zum ändern vom kartenstatus
                string sql = "UPDATE cards SET status=@s WHERE card_id=@id LIMIT 1;";

                // sql befehl erstellen
                using var cmd = new MySqlCommand(sql, conn);

                // neuen status einsetzen
                cmd.Parameters.AddWithValue("@s", newStatus);

                // card_id einsetzen
                cmd.Parameters.AddWithValue("@id", cardId);

                // executeNonQuery gibt zurück wie viele zeilen geändert wurden
                return cmd.ExecuteNonQuery() == 1;
            }
            catch (Exception ex)
            {
                // falls beim update ein fehler auftritt
                MessageBox.Show("Fehler beim Update:\n" + ex.Message);
                return false;
            }
        }

        // button für sperren oder entsperren
        private void ToggleBlock_Click(object sender, RoutedEventArgs e)
        {
            // ausgewählte karte holen
            var selected = GetSelectedCard();

            // wenn keine karte ausgewählt wurde
            if (selected == null)
            {
                MessageBox.Show("Bitte zuerst eine Karte auswählen.");
                return;
            }

            // card_id und alter status aus der auswahl holen
            int cardId = selected.Value.CardId;
            string oldStatus = selected.Value.Status;

            // wenn die karte schon blocked ist -> active
            // sonst -> blocked
            string newStatus = oldStatus.Equals("BLOCKED", StringComparison.OrdinalIgnoreCase)
                ? "ACTIVE"
                : "BLOCKED";

            // status in der datenbank ändern
            if (SetCardStatus(cardId, newStatus))
            {
                LoadCards(); // datagrid neu laden, damit man die änderung direkt sieht
            }
        }
    }
}