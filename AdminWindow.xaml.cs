using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows;

namespace Banking_app
{
    public partial class AdminWindow : Window
    {
        private readonly string _connectionString =
            "server=mysql.pb.bib.de;uid=pbt3h24akr;pwd=zJpyj6GPvtK6;database=pbt3h24akr_Wombank";

        private Admin _adminPage;

        public AdminWindow()
        {
            InitializeComponent();
        }

        private void AdminWindow_Loaded(object sender, RoutedEventArgs e)
        {
            NavigateToUsers();
        }

        private void Users_Click(object sender, RoutedEventArgs e)
        {
            NavigateToUsers();
        }

        private void NavigateToUsers()
        {
            _adminPage = new Admin();                
            AdminFrame.Navigate(_adminPage);      
            LoadUsersIntoPage();                     
        }

        private void LoadUsersIntoPage()
        {
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();

                    string query = @"SELECT user_id, username, role, is_active, created_at
                                     FROM users
                                     ORDER BY user_id;";

                    var adapter = new MySqlDataAdapter(query, conn);
                    var table = new DataTable();
                    adapter.Fill(table);

                    _adminPage?.SetUsersSource(table.DefaultView);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("DB Fehler:\n" + ex.Message);
            }
        }

        public void ToggleActiveForSelectedUser()
        {
            if (_adminPage == null)
                return;

            if (_adminPage.GetSelectedRow() is not DataRowView row)
            {
                MessageBox.Show("Bitte einen Benutzer auswählen.");
                return;
            }

            int userId = Convert.ToInt32(row["user_id"]);
            bool isActive = Convert.ToBoolean(row["is_active"]);
            bool newValue = !isActive;

            if (row["username"].ToString() == "admin")
            {
                MessageBox.Show("Admin kann nicht deaktiviert werden.");
                return;
            }

            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();

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

                LoadUsersIntoPage(); // refresh
            }
            catch (Exception ex)
            {
                MessageBox.Show("Update Fehler:\n" + ex.Message);
            }
        }

        public void ReloadUsers()
        {
            LoadUsersIntoPage();
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            new Login().Show();
            Close();
        }
    }
}
