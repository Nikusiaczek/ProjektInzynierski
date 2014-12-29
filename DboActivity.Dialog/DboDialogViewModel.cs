using AddMariage;
using InsertDialog;
using Microsoft.Practices.Prism.Commands;
using ModifyDialog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DboActivity.Dialog
{
    public class DboDialogViewModel : INotifyPropertyChanged
    {
        private string _windowName;
        private object _data;
        private Model model = new Model();
        private readonly DelegateCommand _insertDialogPopup;
        private readonly DelegateCommand _modifyDialog;
        private readonly DelegateCommand _deleteDialog;
        private string connectionString = "Data Source=AGALAP;Initial Catalog=\"D:\\PROJEKTY VISUAL\\POPULATIONREGISTERING\\POPULATIONREGISTER.MDF\";Integrated Security=True";


        public DboDialogViewModel(string windowName)
        {
            this._windowName = windowName;
            PrepareDataGrid(windowName);
            _insertDialogPopup = new DelegateCommand(AddToDB);
            _modifyDialog = new DelegateCommand(ModifyAndPushToDB);
            _deleteDialog = new DelegateCommand(DeleteAndPushToDB);
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

        public ICommand InsertDialogCommand { get { return _insertDialogPopup; } }
        public ICommand ModifyDialogCommand { get { return _modifyDialog; } }
        public ICommand DeleteDialogCommand { get { return _deleteDialog; } }

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
                                context.Person.Add(FillPerson(insertVM.Pesel, insertVM.FirstName, insertVM.MiddleName, insertVM.LastName, insertVM.DateOfBirth, insertVM.Sex));
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
                                context.Deaths.Add(FillDeath(1, insertVM.Date, insertVM.Pesel));
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
                        {
                            var context = new Entities();
                            AddMarriageViewModel addVM = new AddMarriageViewModel();
                            AddMarriageWindow dialog = new AddMarriageWindow(addVM);
                            bool? res = dialog.ShowDialog();
                            if (res.HasValue && res.Value)
                            {
                                Person person = FillPerson(addVM.Pesel1, addVM.FirstName1, addVM.MiddleName1, addVM.LastName1, addVM.DateOfBirth1, addVM.Sex1),
                                       person1 = FillPerson(addVM.Pesel2, addVM.FirstName2, addVM.MiddleName2, addVM.LastName2, addVM.DateOfBirth2, addVM.Sex2);
                                var personList = context.Person.ToList();
                                Person personToFind = personList.Where(p => p.pesel.Equals(person.pesel)).FirstOrDefault<Person>();
                                Person personToFind1 = personList.Where(p => p.pesel.Equals(person1.pesel)).FirstOrDefault<Person>();
                                if (personToFind == null)
                                {
                                    context.Person.Add(person);
                                }
                                if (personToFind1 == null)
                                {
                                    context.Person.Add(person1);
                                }
                                context.Marriages.Add(FillMariage(addVM.ActNumber, addVM.Pesel1, addVM.Pesel2, addVM.Date));
                                context.SaveChanges();
                                Marriage marriage = new Marriage();
                                marriage.Pesel1 = addVM.Pesel1; marriage.FirstName1 = addVM.FirstName1; marriage.MiddleName1 = addVM.MiddleName1;
                                marriage.LastName1 = addVM.LastName1; marriage.Pesel2 = addVM.Pesel2; marriage.FirstName2 = addVM.FirstName2;
                                marriage.MiddleName2 = addVM.MiddleName2; marriage.LastName2 = addVM.LastName2; marriage.Date = addVM.Date;
                                model.MData.Add(marriage);
                            }
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

        //todo
        public void ModifyAndPushToDB()
        {
            using (SqlConnection db = new SqlConnection(connectionString))
            {
                db.Open();
                switch (WindowName)
                {
                    case "Narodziny":
                        if (SelectedObject != null)
                        {
                            Birth birth = (Birth)SelectedObject;                         
                            ModifyViewModel modVM = new ModifyViewModel();
                            modVM.Pesel = birth.Pesel; modVM.MothersPesel = birth.MothersPesel; modVM.DateOfBirth = birth.Date;
                            ModifyDialogWindow dialog = new ModifyDialogWindow(modVM);
                            bool? res = dialog.ShowDialog();
                            if (res.HasValue && res.Value)
                            {
                                
                            }
                        }
                        else
                        {
                            MessageBox.Show("Zaznacz wiersz!","Błąd!", MessageBoxButton.OK,MessageBoxImage.Error);
                        }
                        break;
                    case "Zgony":
                        if (SelectedObject != null)
                        {
                            Death death = (Death)SelectedObject;
                            ModifyViewModel modVM = new ModifyViewModel();
                            modVM.Pesel = death.Pesel; modVM.Code = death.ActNumber; modVM.DateOfBirth = death.Date;
                            ModifyDialogWindow dialog = new ModifyDialogWindow(modVM);
                            bool? res = dialog.ShowDialog();
                            if (res.HasValue && res.Value)
                            {

                            }
                        }
                        else
                        {
                            MessageBox.Show("Zaznacz wiersz!", "Błąd!", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        break;
                    case "Zameldowanie":
                        if (SelectedObject != null)
                        {
                            Accomodate acc = (Accomodate)SelectedObject;
                            ModifyViewModel modVM = new ModifyViewModel();
                            modVM.Pesel = acc.Pesel;
                            ModifyDialogWindow dialog = new ModifyDialogWindow(modVM);
                            bool? res = dialog.ShowDialog();
                            if (res.HasValue && res.Value)
                            {

                            }
                        }
                        else
                        {
                            MessageBox.Show("Zaznacz wiersz!", "Błąd!", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        break;
                    case "Dane Osobowe":
                        if (SelectedObject != null)
                        {
                            PersonDetails personD = (PersonDetails)SelectedObject;
                            ModifyViewModel modVM = new ModifyViewModel();
                            modVM.Pesel = personD.Pesel; modVM.DateOfBirth = personD.DateOfBirth;
                            ModifyDialogWindow dialog = new ModifyDialogWindow(modVM);
                            bool? res = dialog.ShowDialog();
                            if (res.HasValue && res.Value)
                            {

                            }
                        }
                        else
                        {
                            MessageBox.Show("Zaznacz wiersz!", "Błąd!", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        break;
                    case "Małżeństwa":
                        if (SelectedObject != null)
                        {
                            Marriage marriage = (Marriage)SelectedObject;
                            ModifyViewModel modVM = new ModifyViewModel();
                            modVM.Pesel = marriage.Pesel1; modVM.Date = marriage.Date;
                            ModifyDialogWindow dialog = new ModifyDialogWindow(modVM);
                            bool? res = dialog.ShowDialog();
                            if (res.HasValue && res.Value)
                            {

                            }
                        }
                        else
                        {
                            MessageBox.Show("Zaznacz wiersz!", "Błąd!", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        break;
                }
                db.Close();
            }
        }

        //todo
        public void DeleteAndPushToDB()
        {
            using (SqlConnection db = new SqlConnection(connectionString))
            {
                db.Open();

                if (SelectedObject != null)
                {
                    MessageBoxResult result = MessageBox.Show("Jesteś pewien?", "Usuń", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result.Equals(MessageBoxResult.Yes))
                    {
                        switch (WindowName)
                        {
                            case "Narodziny":

                                break;
                            case "Zgony":

                                break;
                            case "Dane Osobowe":

                                break;
                            case "Małżeństwa":

                                break;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Zaznacz wiersz!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
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
