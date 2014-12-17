using InsertDialog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DboActivity.Dialog
{
    public class DboDialogViewModel : INotifyPropertyChanged
    {
        private string _windowName;
        private string connectionString = "Data Source=AGALAP;Initial Catalog=\"D:\\PROJEKTY VISUAL\\POPULATIONREGISTERING\\POPULATIONREGISTER.MDF\";Integrated Security=True";


        public DboDialogViewModel(string windowName)
        {
            this._windowName = windowName;
            PrepareDataGrid(windowName);
        }

        public bool Inactive
        {
            get { return false; }
        }
        public string WindowName
        {
            get { return _windowName; }
        }

        public object Data { get; set; }

        private void PrepareDataGrid(string windowName)
        {
            using (SqlConnection db = new SqlConnection(connectionString))
            {
                db.Open();
                switch (windowName)
                {
                    case "Dane Osobowe":
                        {
                            var context = new Entities();
                            var data = from p in context.Person
                                       select new
                                       {
                                           p.pesel,
                                           p.firstName,
                                           p.middleName,
                                           p.lastName,
                                           p.dateOfBirth,
                                           p.sex
                                       };
                            Data = data.ToList();
                        }
                        break;
                    case "Narodziny":
                        {
                            var context = new Entities();
                            var data = from p in context.Person
                                       join b in context.Births on p.pesel equals b.pesel
                                       select new
                                       {
                                           p.pesel,
                                           p.firstName,
                                           p.middleName,
                                           p.lastName,
                                           b.date,
                                           b.mothersPesel
                                       };
                            Data = data.ToList();
                        }
                        break;
                    case "Zgony":
                        {
                            var context = new Entities();
                            var data = from p in context.Person
                                       join d in context.Deaths on p.pesel equals d.pesel
                                       select new
                                       {
                                           p.pesel,
                                           p.firstName,
                                           p.middleName,
                                           p.lastName,
                                           d.date,
                                           d.ID
                                       };
                            Data = data.ToList();
                        }
                        break;
                    case "Małżeństwa":
                        {
                            var context = new Entities();
                            var data = from m in context.Marriages
                                       join p in context.Person on m.pesel equals p.pesel
                                       join p1 in context.Person on m.pesel2 equals p1.pesel
                                       select new
                                       {
                                           m.pesel,
                                           p.firstName,
                                           p.middleName,
                                           p.lastName,
                                           m.pesel2,
                                           partnerFirstName = p1.firstName,
                                           partnerMiddleName = p1.middleName,
                                           partnerLastName = p1.lastName,
                                           m.date,
                                           m.anulled,
                                           m.description
                                       };
                            Data = data.ToList();
                        }
                        break;
                    case "Zameldowanie":
                        {
                            var context = new Entities();
                            var data = from p in context.Person
                                       join a in context.Accommodation on p.permanentAddress_ID equals a.ID
                                       join a1 in context.Accommodation on p.temporaryAddress_ID equals a1.ID
                                       select new
                                       {
                                           p.pesel,
                                           p.firstName,
                                           p.lastName,
                                           a.country,
                                           a.city,
                                           a.postCode,
                                           a.street,
                                           a.buildingNumber,
                                           a.flatNumber,
                                           tmpCountry = a1.country,
                                           tmpCity = a1.city,
                                           tmpPCode = a1.postCode,
                                           tmpStreet = a1.street,
                                           tmpBNumber = a1.buildingNumber,
                                           tmpFNumber = a1.flatNumber
                                       };
                            Data = data.ToList();
                        }
                        break;

                }

                db.Close();
            }
        }

        public void AddToDB()
        {

            using (SqlConnection db = new SqlConnection(connectionString))
            {
                db.Open();
                switch (WindowName)
                {
                    case "Narodziny":
                        {
                            var context = new Entities();
                            InsertDialogViewModel insertVM = new InsertDialogViewModel(WindowName);
                            MainWindow dialog = new MainWindow(insertVM);
                            bool? res = dialog.ShowDialog();
                            if (res.HasValue && res.Value)
                            {
                                context.Person.Add(new Person());
                                context.Births.Add(new Births());
                            }
                        }
                        break;
                    case "Zgony":
                        {
                            var context = new Entities();
                            //dialog.showdialog
                            //walidacja
                            Deaths death = new Deaths();
                            context.Deaths.Add(death);
                            var personList = context.Person.ToList();
                            Person personToUpdate = personList.Where(p => p.pesel.Equals(death.pesel)).FirstOrDefault<Person>();
                            personToUpdate.isDead = true;
                        }
                        break;
                    case "Małżeństwa":
                        {
                            var context = new Entities();
                            //dialog.showdialog
                            //walidacja
                            Person person = new Person(), person1 = new Person();
                            var personList = context.Person.ToList();
                            Person personToFind = personList.Where(p => p.pesel.Equals(person.pesel)).FirstOrDefault<Person>();
                            Person personToFind1 = personList.Where(p => p.pesel.Equals(person1.pesel)).FirstOrDefault<Person>();
                            if (personToFind.Equals(null))
                            {
                                context.Person.Add(person);
                            }
                            if (personToFind1.Equals(null))
                            {
                                context.Person.Add(person1);
                            }
                            context.Marriages.Add(new Marriages());
                        }
                        break;
                    case "Dane Osobowe":
                        break;
                }
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
