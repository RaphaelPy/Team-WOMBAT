using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Banking_app
{
    public partial class Login : Window
    {
        // Hardcode test zugang
        private const string TEST_USER = "test";
        private const string TEST_PASSWORD = "1234";

        public Login()
        {
            InitializeComponent();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            if (txtUser.Text == TEST_USER &&
                txtPassword.Password == TEST_PASSWORD)
            {
                // Login Hauptfenster öffnen
                MainWindow main = new MainWindow();
                main.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show(
                    "Benutzername oder Passwort falsch!\n\n" +
                    "Testdaten:\nUser: test\nPasswort: 1234",
                    "Anmeldung fehlgeschlagen",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}
