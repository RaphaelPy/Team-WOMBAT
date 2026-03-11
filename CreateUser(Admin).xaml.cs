using MySql.Data.MySqlClient;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Banking_app
{
    public partial class CreateUser_Admin_ : Page
    {
        private readonly string connectionString =
            "server=mysql.pb.bib.de;uid=pbt3h24akr;pwd=zJpyj6GPvtK6;database=pbt3h24akr_Wombank";

        public CreateUser_Admin_()
        {
            InitializeComponent();
        }

        private void CreateUser_Click(object sender, RoutedEventArgs e)
        {
            string username = tbUsername.Text;
            string password = tbPassword.Password;
            string role = (cbRole.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password) ||
                role == null)
            {
                MessageBox.Show("Bitte alle Felder ausfüllen.");
                return;
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string sql = @"INSERT INTO users
                                   (username, password_hash, role, is_active, created_at)
                                   VALUES
                                   (@username, SHA2(@password,256), @role, 1, NOW());";

                    MySqlCommand cmd = new MySqlCommand(sql, conn);

                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password);
                    cmd.Parameters.AddWithValue("@role", role);

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Benutzer erfolgreich erstellt.");

                tbUsername.Clear();
                tbPassword.Clear();
                cbRole.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Datenbankfehler:\n" + ex.Message);
            }
        }
    }
}