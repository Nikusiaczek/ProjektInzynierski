using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsertDialog
{
    public class InsertDialogViewModel : INotifyPropertyChanged
    {
        private string _windowName;

        public decimal Pesel { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public bool Sex { get; set; }
        public DateTime Date { get; set; }
        public decimal MothersPesel { get; set; }

        public InsertDialogViewModel(string windowName)
        {
            _windowName = windowName;
        }

        public System.Windows.Visibility ChangeVisibility
        {
            get
            {
                if (_windowName.Equals("Narodziny") || _windowName.Equals("Zgony"))
                {
                    return System.Windows.Visibility.Visible;
                }
                return System.Windows.Visibility.Hidden;
            }
        }

        public System.Windows.Visibility MotherVisibility
        {
            get
            {
                if (_windowName.Equals("Narodziny"))
                {
                    return System.Windows.Visibility.Visible;
                }
                return System.Windows.Visibility.Hidden;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
