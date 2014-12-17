using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsertDialog
{
    public class InsertDialogViewModel: INotifyPropertyChanged
    {
        public enum Visibility { Visible = 0, Hidden = 1 }
        private class DataFromForm
        {
            public decimal Pesel { get; set; }
            public string FirstName { get; set; }
            public string MiddleName { get; set; }
            public string LastName { get; set; }
            public DateTime DateOfBirth { get; set; }
            public bool Sex { get; set; }
            public DateTime Date { get; set; }
            public decimal MothersPesel { get; set; }
        }

        
        public InsertDialogViewModel(string windowName)
        {

        }

        public Visibility ChangeVisibility { get; set; } 

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
