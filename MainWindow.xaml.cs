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
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();

                    // Nur sinnvolle Spalten anzeigen (nicht alle)
                    string query = @"SELECT user_id, username, role, is_active, created_at
                                     FROM users
                                     ORDER BY user_id;";

                    var adapter = new MySqlDataAdapter(query, conn);
                    var table = new DataTable();
                    adapter.Fill(table);

                    dgUsers.ItemsSource = table.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("DB Fehler:\n" + ex.Message);
            }
        }

        private void ToggleActive_Click(object sender, RoutedEventArgs e)
        {
            if (dgUsers.SelectedItem is not DataRowView row)
            {
                MessageBox.Show("Bitte einen Benutzer auswählen.");
                return;
            }

            int userId = Convert.ToInt32(row["user_id"]);
            bool isActive = Convert.ToBoolean(row["is_active"]);
            bool newValue = !isActive;

            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();

                    
                    if (row["username"].ToString() == "admin")
                    {
                        MessageBox.Show("Admin kann nicht deaktiviert werden.");
                        return;
                    }

                    string sql = @"UPDATE users 
                                   SET is_active = @active 
                                   WHERE user_id = @id;";

                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@active", newValue);
                        cmd.Parameters.AddWithValue("@id", userId);
                        cmd.ExecuteNonQuery();
                    }
                }

                LoadData(); // refresh
            }
            catch (Exception ex)
            {
                MessageBox.Show("Update Fehler:\n" + ex.Message);
            }
        }
    }
}
