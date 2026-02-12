using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows;

namespace Banking_app
{
    public partial class MainWindow : Window
    {
        private readonly string _connectionString =
            "server=mysql.pb.bib.de;uid=pbt3h24akr;pwd=zJpyj6GPvtK6;database=pbt3h24akr_Wombank";

        public MainWindow()
        {
            InitializeComponent();
            LoadData(); // beim Start laden
        }

        private void LoadData()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();

                    // HIER Tabellenname anpassen falls nötig
                    string query = "SELECT * FROM users";

                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                    DataTable table = new DataTable();
                    adapter.Fill(table);

                    dgUsers.ItemsSource = table.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("DB Fehler:\n" + ex.Message);
            }
        }
    }
}
