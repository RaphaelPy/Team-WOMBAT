using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Banking_app
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly string _connectionString = 
            "server=mysql.pb.bib.de;uid=pbt3h24akr;pwd=zJpyj6GPvtK6;database=pbt3h24akr_Wombank";

        public MainWindow()
        {
            InitializeComponent();
        }
    }
}