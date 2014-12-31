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
using System.Windows.Shapes;

namespace ModifyDialog
{
    /// <summary>
    /// Interaction logic for Modify_Dialog_b.xaml
    /// </summary>
    public partial class Modify_Dialog_b : Window
    {
        public Modify_Dialog_b(ModifyViewModel modifyVM)
        {
            InitializeComponent();
            this.DataContext = modifyVM;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
