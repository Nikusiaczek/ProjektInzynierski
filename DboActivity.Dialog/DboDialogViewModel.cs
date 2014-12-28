using InsertDialog;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private object _data;
        private Model model = new Model();
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

        public object Data 
        {
            get { return _data; }
            set { _data = value; }
        }

        public object SelectedObject
        {
            get;
            set;
        }
        public bool UnlockButton 
        {
            get
            {
                if (SelectedObject == null)
                {
                    return false;
                }
                return true;
            }  
        }
        


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
                            model.ShowPersonalData();
                            Data = model.PData;
                        }                       
                        break;
                    case "Narodziny":
                        {
                            model.ShowBirth();
                            Data = model.BData;
                        }
                        break;
                    case "Zgony":
                        {
                            model.ShowDeath();
                            Data = model.DData;
                        }
                        break;
                    case "Małżeństwa":
                        {
                            model.ShowMarriage();
                            Data = model.MData;
                        }
                        break;
                    case "Zameldowanie":
                        {
                            model.ShowAccomodation();
                            Data = model.AData;
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
                                Birth birth = new Birth();
                                birth.Pesel = insertVM.Pesel; birth.FirstName = insertVM.FirstName; birth.MiddleName = insertVM.MiddleName;
                                birth.LastName = insertVM.LastName; birth.Date = insertVM.DateOfBirth; birth.MothersPesel = insertVM.MothersPesel;
                                model.BData.Add(birth);
                                NotifyPropertyChanged("Data");

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
                                Death death = new Death();
                                death.FirstName = insertVM.FirstName; death.LastName = insertVM.LastName; death.MiddleName = insertVM.MiddleName;
                                death.Pesel = insertVM.Pesel; death.Date = insertVM.Date;
                                model.DData.Add(death);
                                NotifyPropertyChanged("Data");
                            }
                        }
                        break;
                    case "Małżeństwa":
                        //{
                        //    var context = new Entities();
                        //    //dialog.showdialog
                        //    //walidacja
                        //    //Person person = FillPerson(), person1 = FillPerson();
                        //    var personList = context.Person.ToList();
                        //    Person personToFind = personList.Where(p => p.pesel.Equals(person.pesel)).FirstOrDefault<Person>();
                        //    Person personToFind1 = personList.Where(p => p.pesel.Equals(person1.pesel)).FirstOrDefault<Person>();
                        //    if (personToFind.Equals(null))
                        //    {
                        //        context.Person.Add(person);
                        //    }
                        //    if (personToFind1.Equals(null))
                        //    {
                        //        context.Person.Add(person1);
                        //    }
                        //    context.Marriages.Add(new Marriages());
                        //    context.SaveChanges();
                        //}
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
                            PersonDetails persona = new PersonDetails();
                            persona.Pesel = insertVM.Pesel; persona.FirstName = insertVM.FirstName; persona.MiddleName = insertVM.MiddleName;
                            persona.LastName = insertVM.LastName; persona.IsMale = insertVM.Sex; persona.DateOfBirth = insertVM.DateOfBirth;
                            model.PData.Add(persona);
                            NotifyPropertyChanged("Data");
                        }
                        break;
                }
            }
        }

        public void ModifyAndPushToDB()
        {
            using (SqlConnection db = new SqlConnection(connectionString))
            {
                db.Open();
                switch (WindowName)
                {
                    case "Narodziny":

                        break;
                    case "Zgony":
                        break;
                    case "Zameldowanie":
                        break;
                    case "Dane Osobowe":
                        break;
                    case "Małżeństwa":
                        break;
                }
                db.Close();
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
