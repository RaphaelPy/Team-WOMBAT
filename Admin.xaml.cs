using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace Banking_app
{
    public partial class Admin : Page
    {
        public Admin()
        {
            InitializeComponent();
        }

        // Window setzt die Source
        public void SetUsersSource(DataView view)
        {
            dgUsers.ItemsSource = view;
        }

        // Window braucht Zugriff auf Selection
        public DataRowView GetSelectedRow()
        {
            return dgUsers.SelectedItem as DataRowView;
        }

        // Buttons rufen Window-Methoden auf
        private void ToggleActive_Click(object sender, RoutedEventArgs e)
        {
            if (Window.GetWindow(this) is AdminWindow win)
                win.ToggleActiveForSelectedUser();
        }

        private void Reload_Click(object sender, RoutedEventArgs e)
        {
            if (Window.GetWindow(this) is AdminWindow win)
                win.ReloadUsers();
        }
    }
}
