using System.Windows.Controls;

namespace Banking_app
{
    public partial class user : Page
    {
        public user()
        {
            InitializeComponent();
        }

        //  Window kann Text setzen
        public void SetWelcomeText(string text)
        {
            txtWelcome.Text = text;
        }
    }
}
