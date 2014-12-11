using DboActivity.Dialog;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProjInzynierski
{
    public class ShellViewModel: INotifyPropertyChanged
    {
        private readonly DelegateCommand _dialogPopupCommand;
        private string dbName;
        private string connectionString;

        public ShellViewModel()
        {
            _dialogPopupCommand = new DelegateCommand(Popup, CanConnect);
        }

        private void Popup()
        {
            //Console.WriteLine("Wszedłem!");
            DboDialogPopup dialogPopup = new DboDialogPopup(new DboDialogViewModel());
            dialogPopup.Show();
        }

        private bool CanConnect()
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
