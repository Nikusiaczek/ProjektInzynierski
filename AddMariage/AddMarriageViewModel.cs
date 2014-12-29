using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddMariage
{
    public class AddMarriageViewModel: INotifyPropertyChanged
    {
        public decimal Pesel1 { get; set; }
        public decimal Pesel2 { get; set; }
        public string FirstName1 { get; set; }
        public string FirstName2 { get; set; }
        public string MiddleName1 { get; set; }
        public string MiddleName2 { get; set; }
        public string LastName1 { get; set; }
        public string LastName2 { get; set; }
        public DateTime DateOfBirth1 { get; set; }
        public DateTime DateOfBirth2 { get; set; }
        public bool Sex1 { get; set; }
        public bool Sex2 { get; set; }
        public DateTime Date { get; set; }
        public int ActNumber { get; set; }

        
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
