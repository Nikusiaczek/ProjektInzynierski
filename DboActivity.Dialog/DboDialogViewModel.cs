using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DboActivity.Dialog
{
    public class DboDialogViewModel: INotifyPropertyChanged
    {
        private string _windowName;

        public DboDialogViewModel(string windowName)
        {
            this._windowName = windowName;
        }

        public string WindowName 
        {
            get { return _windowName; } 
        }
   
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
