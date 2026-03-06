using MySql.Data.MySqlClient; // brauche ich für die verbindung zur mysql datenbank
using System; // grundfunktionen von c#
using System.Data; // brauche ich für datatable
using System.Windows; // für wpf elemente wie messagebox
using System.Windows.Controls; // für page, datagrid, buttons usw.

namespace Banking_app.userpages
{
    public partial class TransaktionenPage : Page // seite die alle transaktionen anzeigt
    {
        // speichert die datenbankverbindung
        private readonly string _connectionString;

        // speichert den aktuell eingeloggten benutzernamen
        private readonly string _username;

        // datatable zum speichern der geladenen daten
        private DataTable _data;

        // konstruktor von der seite
        public TransaktionenPage(string connectionString, string username)
        {
            InitializeComponent(); // lädt das xaml design

            _connectionString = connectionString; // verbindung speichern
            _username = username; // username speichern

            // wenn die seite geladen wird -> transfers laden
            Loaded += (_, __) => LoadTransfers();
        }

        // button: daten neu laden
        private void Reload_Click(object sender, RoutedEventArgs e)
        {
            LoadTransfers(); // transaktionen erneut aus der datenbank laden
        }

        // button: alle transaktionen anzeigen
        private void ShowAll_Click(object sender, RoutedEventArgs e)
        {
            // wenn noch keine daten geladen wurden -> abbrechen
            if (_data == null) return;

            // datatable komplett im datagrid anzeigen
            dgTransfers.ItemsSource = _data.DefaultView;
        }

        // button: nur fehlgeschlagene transaktionen anzeigen
        private void ShowOnlyFailed_Click(object sender, RoutedEventArgs e)
        {
            // wenn keine daten geladen wurden -> abbrechen
            if (_data == null) return;

            // dataview erstellen
            var view = _data.DefaultView;

            // filter setzen -> nur status "failed"
            view.RowFilter = "status = 'failed'";

            // gefilterte daten im datagrid anzeigen
            dgTransfers.ItemsSource = view;
        }

        // lädt alle transaktionen vom user aus der datenbank
        private void LoadTransfers()
        {
            try
            {
                // verbindung zur datenbank erstellen
                using var conn = new MySqlConnection(_connectionString);
                conn.Open(); // verbindung öffnen

                // sql query um eingehende und ausgehende überweisungen zu laden
                string sql = @"
            -- OUT: überweisungen die von meinem konto weggehen
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

            -- IN: überweisungen die zu meinem konto kommen
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

                // sql befehl erstellen
                using var cmd = new MySqlCommand(sql, conn);

                // username einsetzen
                cmd.Parameters.AddWithValue("@u", _username);

                // datatable erstellen
                var dt = new DataTable();

                // daten aus der datenbank laden
                using (var ad = new MySqlDataAdapter(cmd))
                {
                    ad.Fill(dt);
                }

                // daten im datagrid anzeigen
                dgTransfers.ItemsSource = dt.DefaultView;
            }
            catch (Exception ex)
            {
                // falls ein fehler beim laden passiert
                MessageBox.Show("Fehler beim Laden:\n" + ex.Message);
            }
        }
    }
}