using InsertDialog;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DboActivity.Dialog
{
    public class DboDialogViewModel : INotifyPropertyChanged
    {
        private string _windowName;
        private readonly DelegateCommand _insertDialogPopup;
        private string connectionString = "Data Source=AGALAP;Initial Catalog=\"D:\\PROJEKTY VISUAL\\POPULATIONREGISTERING\\POPULATIONREGISTER.MDF\";Integrated Security=True";


        public DboDialogViewModel(string windowName)
        {
            this._windowName = windowName;
            PrepareDataGrid(windowName);
            _insertDialogPopup = new DelegateCommand(AddToDB);
        }

        public bool Inactive
        {
            get 
            {
                if (WindowName.Equals("Zameldowanie"))
                {
                    return false;
                }
                return true;
            }
        }
        public string WindowName
        {
            get { return _windowName; }
        }

        public object Data { get; set; }

        public ICommand InsertDialogCommand { get { return _insertDialogPopup; } }

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
                            InsertDialogWindow dialog = new InsertDialogWindow(insertVM);
                            bool? res = dialog.ShowDialog();
                            if (res.HasValue && res.Value)
                            {
                                context.Person.Add(FillPerson(insertVM.Pesel,insertVM.FirstName,insertVM.MiddleName, insertVM.LastName,insertVM.DateOfBirth,insertVM.Sex));                              
                                context.Births.Add(FillBirth(1, insertVM.Date, insertVM.Pesel, insertVM.MothersPesel));
                                context.SaveChanges();
                            }
                        }
                        break;
                    case "Zgony":
                        {
                            var context = new Entities();
                            InsertDialogViewModel insertVM = new InsertDialogViewModel(WindowName);
                            InsertDialogWindow dialog = new InsertDialogWindow(insertVM);
                            bool? res = dialog.ShowDialog();
                            if (res.HasValue && res.Value)
                            {
                                context.Deaths.Add(FillDeath(1,insertVM.Date, insertVM.Pesel));
                                var personList = context.Person.ToList();
                                Person personToUpdate = personList.Where(p => p.pesel.Equals(insertVM.Pesel)).FirstOrDefault<Person>();
                                personToUpdate.isDead = true;
                                context.SaveChanges();
                            }
                        }
                        break;
                    case "Małżeństwa":
                        {
                            var context = new Entities();
                            //dialog.showdialog
                            //walidacja
                            Person person = FillPerson(), person1 = FillPerson();
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
                            context.SaveChanges();
                        }
                        break;
                    case "Dane Osobowe":
                        {
                            var context = new Entities();
                            InsertDialogViewModel insertVM = new InsertDialogViewModel(WindowName);
                            InsertDialogWindow dialog = new InsertDialogWindow(insertVM);
                            bool? res = dialog.ShowDialog();
                            if (res.HasValue && res.Value)
                            {
                                context.Person.Add(FillPerson(insertVM.Pesel, insertVM.FirstName, insertVM.MiddleName, insertVM.LastName, insertVM.DateOfBirth, insertVM.Sex));
                            }
                            context.SaveChanges();
                        }
                        break;
                }
            }
        }

        public Person FillPerson(decimal pesel, string firstName, string middleName, string lastName, DateTime birth, bool sex)
        {
            Person person = new Person();
            person.pesel = pesel;
            person.firstName = firstName;
            person.middleName = middleName;
            person.lastName = lastName;
            person.dateOfBirth = birth;
            person.sex = sex;
            person.temporaryAddress_ID = 0;
            person.permanentAddress_ID = 0;
            person.isDead = false;
            return person;
        }

        public Births FillBirth(int id, DateTime date, decimal pesel, decimal mother)
        {
            Births birth = new Births();
            birth.ID = id;
            birth.pesel = pesel;
            birth.date = date;
            birth.mothersPesel = mother;
            return birth;
        }

        public Deaths FillDeath(int id, DateTime date, decimal pesel)
        {
            Deaths death = new Deaths();
            death.ID = id;
            death.date = date;
            death.pesel = pesel;
            return death;
        }

        public Marriages FillMariage(int id, decimal pesel, decimal pesel2, DateTime date)
        {
            Marriages marriage = new Marriages();
            marriage.ID = id;
            marriage.pesel = pesel;
            marriage.pesel2 = pesel2;
            marriage.date = date;
            return marriage;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
