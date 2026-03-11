using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace Banking_app
{
    public partial class DeleteUser_Admin_ : Page
    {
        private readonly string connectionString =
            "server=mysql.pb.bib.de;uid=pbt3h24akr;pwd=zJpyj6GPvtK6;database=pbt3h24akr_Wombank";

        public DeleteUser_Admin_()
        {
            InitializeComponent();
            LoadUsers();
        }

        private void LoadUsers()
        {
            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string sql = "SELECT user_id, username, role, is_active, created_at FROM users";

                    MySqlDataAdapter adapter = new MySqlDataAdapter(sql, conn);

                    DataTable table = new DataTable();
                    adapter.Fill(table);

                    dgUsers.ItemsSource = table.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler:\n" + ex.Message);
            }
        }

        private void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (dgUsers.SelectedItem is not DataRowView row)
            {
                MessageBox.Show("Bitte einen Benutzer auswählen.");
                return;
            }

            int userId = Convert.ToInt32(row["user_id"]);
            string username = row["username"].ToString();

            if (username == "admin")
            {
                MessageBox.Show("Admin kann nicht gelöscht werden.");
                return;
            }

            if (MessageBox.Show($"User '{username}' wirklich löschen?",
                "Bestätigung",
                MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;

            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string sql = "DELETE FROM users WHERE user_id=@id";

                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@id", userId);

                    cmd.ExecuteNonQuery();
                }

                LoadUsers();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler:\n" + ex.Message);
            }
        }
    }
}