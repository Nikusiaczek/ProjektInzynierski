using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DboActivity.Dialog
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class DboDialogPopup : Window
    {
        public DboDialogPopup(DboDialogViewModel dboDialogVM)
        {
            InitializeComponent();
            this.DataContext = dboDialogVM;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.BirthDataGrid.Items.Refresh();
        }

    }
}
