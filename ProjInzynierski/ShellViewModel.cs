using DboActivity.Dialog;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace ProjInzynierski
{
    public class ShellViewModel: INotifyPropertyChanged
    {
        private readonly DelegateCommand<object> _dialogPopupCommand;
        private string dbName;
        private string connectionString;

        public ShellViewModel()
        {
            _dialogPopupCommand = new DelegateCommand<object>(Popup, CanConnect);
        }

        private void Popup(object sender)
        {
            Button button = sender as Button;
            string content = button.Content.ToString();
            DboDialogPopup dialogPopup = new DboDialogPopup(new DboDialogViewModel(content));
            dialogPopup.Show();
            
        }

        private bool CanConnect(object sender)
        {
            return true;
        }

        public ICommand PopupCommand { get { return _dialogPopupCommand; } }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
