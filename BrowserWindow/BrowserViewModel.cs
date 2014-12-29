using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BrowserWindow
{
    public class BrowserViewModel: INotifyPropertyChanged
    {
        private readonly DelegateCommand _browse;

        public BrowserViewModel()
        {
            _browse = new DelegateCommand(Browse);
        }

        public ICommand BrowseCommand { get { return _browse; } }

        public string TableChoice { get; set; }
        public string SearchPhrase { get; set; }

        public void Browse()
        {

        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
