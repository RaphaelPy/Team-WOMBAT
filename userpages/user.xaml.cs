using MySql.Data.MySqlClient; // brauche ich für die verbindung mit mysql
using System; // grundfunktionen von c#
using System.Data; // brauche ich für datatable
using System.Windows; // für wpf sachen wie fenster
using System.Windows.Controls; // für page, button, datagrid usw.

namespace Banking_app
{
    public partial class user : Page // dashboard seite vom benutzer
    {
        // speichert die datenbankverbindung
        private string _connectionString;

        // speichert den benutzernamen vom eingeloggten user
        private string _username;

        // konstruktor von der seite
        public user()
        {
            InitializeComponent(); // lädt das xaml design
        }

        // bekommt verbindung und username vom hauptfenster
        public void SetConnection(string connectionString, string username)
        {
            _connectionString = connectionString; // verbindung speichern
            _username = username; // username speichern

            LoadDashboard(); // dashboard daten laden
        }

        // setzt den willkommens text oben auf der seite
        public void SetWelcomeText(string text)
        {
            txtWelcome.Text = text; // text anzeigen
        }

        // lädt die daten für das dashboard
        private void LoadDashboard()
        {
            try
            {
                // verbindung zur datenbank erstellen
                using var conn = new MySqlConnection(_connectionString);
                conn.Open(); // verbindung öffnen

                // gesamtguthaben vom user laden
                using (var cmd = new MySqlCommand(@"
                    SELECT SUM(balance)
                    FROM accounts a
                    JOIN users u ON u.user_id = a.user_id
                    WHERE u.username=@u;", conn))
                {
                    // username in die query einsetzen
                    cmd.Parameters.AddWithValue("@u", _username);

                    // einzelnen wert holen
                    var result = cmd.ExecuteScalar();

                    // wenn nichts gefunden wurde -> 0, sonst in decimal umwandeln
                    decimal balance = result == DBNull.Value ? 0 : Convert.ToDecimal(result);

                    // kontostand im textfeld anzeigen
                    txtBalance.Text = $"{balance:0.00} €";
                }

                // letzte transaktionen vom user laden
                using (var cmd = new MySqlCommand(@"
                    SELECT created_at, to_name, amount, status
                    FROM transfers t
                    JOIN accounts a ON a.account_id = t.from_account_id
                    JOIN users u ON u.user_id = a.user_id
                    WHERE u.username=@u
                    ORDER BY created_at DESC
                    LIMIT 5;", conn))
                {
                    // username wieder einsetzen
                    cmd.Parameters.AddWithValue("@u", _username);

                    // datatable erstellen
                    var dt = new DataTable();

                    // daten aus der datenbank in die datatable laden
                    new MySqlDataAdapter(cmd).Fill(dt);

                    // daten im datagrid anzeigen
                    dgRecentTransfers.ItemsSource = dt.DefaultView;
                }
            }
            catch
            {
                // falls ein fehler passiert, wird hier nichts angezeigt
            }
        }

        // button führt zur konto seite
        private void Gotokonto(object sender, RoutedEventArgs e)
        {
            // aktuelles fenster holen und in mainwindow umwandeln
            var main = Window.GetWindow(this) as MainWindow;

            // wenn nichts gefunden wurde -> abbrechen
            if (main == null) return; // passiert nur im designer

            // zur konto seite wechseln
            main.MainFrame.Navigate(new Banking_app.userpages.KontoPage(_connectionString, _username));
        }

        // button führt zur transaktionen seite
        private void gototransaktionen(object sender, RoutedEventArgs e)
        {
            // hauptfenster holen
            var main = Window.GetWindow(this) as MainWindow;

            // wenn kein hauptfenster da ist -> abbrechen
            if (main == null) return;

            // zur transaktionen seite wechseln
            main.MainFrame.Navigate(new Banking_app.userpages.TransaktionenPage(_connectionString, _username));
        }

        // button führt zur karten seite
        private void gotokarten(object sender, RoutedEventArgs e)
        {
            // hauptfenster holen
            var main = Window.GetWindow(this) as MainWindow;

            // wenn kein hauptfenster da ist -> abbrechen
            if (main == null) return;

            // zur karten seite wechseln
            main.MainFrame.Navigate(new Banking_app.userpages.KartenPage(_connectionString, _username));
        }
    }
}