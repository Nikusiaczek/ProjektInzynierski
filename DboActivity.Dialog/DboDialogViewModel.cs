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
using System.Windows.Controls;
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
        private readonly DelegateCommand<object> _browser;
        private string connectionString = "Data Source=AGALAP;Initial Catalog=\"D:\\PROJEKTY VISUAL\\POPULATIONREGISTERING\\POPULATIONREGISTER.MDF\";Integrated Security=True";


        public DboDialogViewModel(string windowName)
        {
            this._windowName = windowName;
            PrepareDataGrid(windowName);
            model.CreateColumnSet(windowName);
            ColumnSet = model.ColumnSet;
            _insertDialogPopup = new DelegateCommand(AddToDB);
            _modifyDialog = new DelegateCommand(ModifyAndPushToDB);
            _deleteDialog = new DelegateCommand(DeleteAndPushToDB);
            _browser = new DelegateCommand<object>(Browse);
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

        public string SelectedColumn { get; set; }
        public string SearchPhrase { get; set; }
        public List<string> ColumnSet { get; set; }

        public ICommand InsertDialogCommand { get { return _insertDialogPopup; } }
        public ICommand ModifyDialogCommand { get { return _modifyDialog; } }
        public ICommand DeleteDialogCommand { get { return _deleteDialog; } }
        public ICommand BrowserCommand { get { return _browser; } }

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

        private void AddToDB()
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

        private void ModifyAndPushToDB()
        {
            using (SqlConnection db = new SqlConnection(connectionString))
            {
                db.Open();
                var context = new Entities();
                switch (WindowName)
                {
                    case "Narodziny":
                        if (SelectedObject != null)
                        {
                            Birth birth = (Birth)SelectedObject;
                            ModifyViewModel modVM = new ModifyViewModel();
                            modVM.Pesel = birth.Pesel; modVM.MothersPesel = birth.MothersPesel; modVM.DateOfBirth = birth.Date;
                            modVM.FirstName = birth.FirstName; modVM.MiddleName = birth.MiddleName; modVM.LastName = birth.LastName;
                            Modify_Dialog_b dialog = new Modify_Dialog_b(modVM);
                            bool? res = dialog.ShowDialog();
                            if (res.HasValue && res.Value)
                            {
                                var birthList = context.Births.ToList();
                                Births birthToMod = birthList.Where(o => o.pesel.Equals(modVM.Pesel)).FirstOrDefault<Births>();
                                birthToMod.mothersPesel = modVM.MothersPesel;
                                birthToMod.date = modVM.DateOfBirth;
                                context.SaveChanges();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Zaznacz wiersz!", "Błąd!", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        break;
                    case "Zgony":
                        if (SelectedObject != null)
                        {
                            Death death = (Death)SelectedObject;
                            ModifyViewModel modVM = new ModifyViewModel();
                            modVM.Pesel = death.Pesel; modVM.Code = death.ActNumber; modVM.DateOfBirth = death.Date;
                            modVM.FirstName = death.FirstName; modVM.MiddleName = death.MiddleName; modVM.LastName = death.LastName;
                            ModifyDialog_d dialog = new ModifyDialog_d(modVM);
                            bool? res = dialog.ShowDialog();
                            if (res.HasValue && res.Value)
                            {
                                var deathList = context.Deaths.ToList();
                                Deaths deathToMod = deathList.Where(o => o.pesel.Equals(modVM.Pesel)).FirstOrDefault<Deaths>();
                                deathToMod.ID = modVM.Code;
                                deathToMod.date = modVM.DateOfBirth;
                                context.SaveChanges();
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
                            modVM.Pesel = acc.Pesel; modVM.FirstName = acc.FirstName; modVM.LastName = acc.LastName;
                            ModifyDialog_a dialog = new ModifyDialog_a(modVM);
                            bool? res = dialog.ShowDialog();
                            if (res.HasValue && res.Value)
                            {
                                var accList = context.Person.ToList();
                                Person accToMod = accList.Where(o => o.pesel.Equals(modVM.Pesel)).FirstOrDefault<Person>();
                                accToMod.permanentAddress_ID = modVM.IDPerm;
                                accToMod.temporaryAddress_ID = modVM.IDTemp;
                                context.SaveChanges();                                
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
                            modVM.Pesel = personD.Pesel; modVM.DateOfBirth = personD.DateOfBirth; personD.IsMale = modVM.Sex;
                            modVM.FirstName = personD.FirstName; modVM.MiddleName = personD.MiddleName; modVM.LastName = personD.LastName;
                            ModifyDialogWindow_p dialog = new ModifyDialogWindow_p(modVM);
                            bool? res = dialog.ShowDialog();
                            if (res.HasValue && res.Value)
                            {
                                var personList = context.Person.ToList();
                                Person personToMod = personList.Where(o => o.pesel.Equals(modVM.Pesel)).FirstOrDefault<Person>();
                                personToMod.firstName = modVM.FirstName; personToMod.middleName = modVM.MiddleName;
                                personToMod.lastName = modVM.LastName; personToMod.sex = modVM.Sex;
                                context.SaveChanges();
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
                            modVM.Pesel = marriage.Pesel1; modVM.Date = marriage.Date; modVM.Pesel1 = marriage.Pesel2; modVM.Anulled = marriage.Anulled;
                            modVM.FirstName = marriage.FirstName1; modVM.MiddleName = marriage.MiddleName1; modVM.LastName = marriage.LastName1;
                            modVM.FirstName1 = marriage.FirstName2; modVM.MiddleName1 = marriage.MiddleName2; modVM.LastName1 = marriage.LastName2;
                            modVM.Description = marriage.Description;
                            ModifyDialog_m dialog = new ModifyDialog_m(modVM);
                            bool? res = dialog.ShowDialog();
                            if (res.HasValue && res.Value)
                            {
                                var marriageList = context.Marriages.ToList();
                                Marriages marriageToMod = marriageList.Where(o => o.pesel.Equals(modVM.Pesel)).FirstOrDefault<Marriages>();
                                marriageToMod.date = modVM.Date; marriageToMod.description = modVM.Description; marriageToMod.anulled = modVM.Anulled;
                                context.SaveChanges();
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

        private void DeleteAndPushToDB()
        {
            using (SqlConnection db = new SqlConnection(connectionString))
            {
                db.Open();
                if (SelectedObject != null)
                {
                    MessageBoxResult result = MessageBox.Show("Jesteś pewien?", "Usuń", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result.Equals(MessageBoxResult.Yes))
                    {
                        var context = new Entities();
                        switch (WindowName)
                        {
                            case "Narodziny":
                                Birth birth = (Birth)SelectedObject;
                                var birthList = context.Births.ToList();
                                Births birthToFind = birthList.Where(p => p.pesel.Equals(birth.Pesel)).FirstOrDefault<Births>();
                                context.Births.Remove(birthToFind);
                                context.SaveChanges();
                                model.BData.Remove(birth);
                                NotifyPropertyChanged("Data");
                                break;
                            case "Zgony":
                                Death death = (Death)SelectedObject;
                                var deathList = context.Deaths.ToList();
                                Deaths deathToFind = deathList.Where(p => p.pesel.Equals(death.Pesel)).FirstOrDefault<Deaths>();
                                context.Deaths.Remove(deathToFind);
                                context.SaveChanges();
                                model.DData.Remove(death);
                                NotifyPropertyChanged("Data");
                                break;
                            case "Dane Osobowe":
                                PersonDetails person = (PersonDetails)SelectedObject;
                                var personList = context.Person.ToList();
                                Person personToFind = personList.Where(p => p.pesel.Equals(person.Pesel)).FirstOrDefault<Person>();
                                context.Person.Remove(personToFind);
                                context.SaveChanges();
                                model.PData.Remove(person);
                                NotifyPropertyChanged("Data");
                                break;
                            case "Małżeństwa":
                                Marriage marriage = (Marriage)SelectedObject;
                                var marriageList = context.Marriages.ToList();
                                Marriages marriageToFind = marriageList.Where(p => p.pesel.Equals(marriage.Pesel1) || p.pesel.Equals(marriage.Pesel2)).FirstOrDefault<Marriages>();
                                context.Marriages.Remove(marriageToFind);
                                context.SaveChanges();
                                model.MData.Remove(marriage);
                                NotifyPropertyChanged("Data");
                                break;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Zaznacz wiersz!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                db.Close();
            }
        }

        private void Browse(object sender)
        {
            ComboBox combo = sender as ComboBox;
            if (combo != null && SearchPhrase != null)
            {
                SelectedColumn = combo.SelectedValue.ToString();
                switch (WindowName)
                {
                    case "Dane Osobowe":
                        IEnumerable<PersonDetails> listaP = new ObservableCollection<PersonDetails>();
                        switch (SelectedColumn)
                        {
                            case "Pesel":
                                decimal pesel = Decimal.Parse(SearchPhrase);
                                listaP = model.PData.Where(o => o.Pesel == pesel);
                                Data = listaP;
                                NotifyPropertyChanged("Data");
                                break;
                            case "Imię":
                                listaP = model.PData.Where(o => o.FirstName == SearchPhrase);
                                Data = listaP;
                                NotifyPropertyChanged("Data");
                                break;
                            case "Drugie Imię":
                                listaP = model.PData.Where(o => o.MiddleName == SearchPhrase);
                                Data = listaP;
                                NotifyPropertyChanged("Data");
                                break;
                            case "Nazwisko":
                                listaP = model.PData.Where(o => o.LastName == SearchPhrase);
                                Data = listaP;
                                NotifyPropertyChanged("Data");
                                break;
                            case "Data Urodzenia":
                                DateTime date = DateTime.Parse(SearchPhrase);
                                listaP = model.PData.Where(o => o.DateOfBirth == date);
                                Data = listaP;
                                NotifyPropertyChanged("Data");
                                break;
                            case "Czy Mężczyzna":
                                bool ismale = Boolean.Parse(SearchPhrase);
                                listaP = model.PData.Where(o => o.IsMale == ismale);
                                Data = listaP;
                                NotifyPropertyChanged("Data");
                                break;
                        }
                        break;
                    case "Narodziny":
                        IEnumerable<Birth> listaB = new ObservableCollection<Birth>();
                        switch (SelectedColumn)
                        {
                            case "Pesel":
                                decimal pesel = Decimal.Parse(SearchPhrase);
                                listaB = model.BData.Where(o => o.Pesel == pesel);
                                Data = listaB;
                                NotifyPropertyChanged("Data");
                                break;
                            case "Imię":
                                listaB = model.BData.Where(o => o.FirstName == SearchPhrase);
                                Data = listaB;
                                NotifyPropertyChanged("Data");
                                break;
                            case "Drugie Imię":
                                listaB = model.BData.Where(o => o.MiddleName == SearchPhrase);
                                Data = listaB;
                                NotifyPropertyChanged("Data");
                                break;
                            case "Nazwisko":
                                listaB = model.BData.Where(o => o.LastName == SearchPhrase);
                                Data = listaB;
                                NotifyPropertyChanged("Data");
                                break;
                            case "Data":
                                DateTime date = DateTime.Parse(SearchPhrase);
                                listaB = model.BData.Where(o => o.Date == date);
                                Data = listaB;
                                NotifyPropertyChanged("Data");
                                break;
                            case "Pesel Matki":
                                decimal mompesel = Decimal.Parse(SearchPhrase);
                                listaB = model.BData.Where(o => o.Pesel == mompesel);
                                Data = listaB;
                                NotifyPropertyChanged("Data");
                                break;
                        }
                        break;
                    case "Małżeństwa":
                        IEnumerable<Marriage> listaM = new ObservableCollection<Marriage>();
                        switch (SelectedColumn)
                        {
                            case "Pesel1":
                                decimal pesel1 = Decimal.Parse(SearchPhrase);
                                listaM = model.MData.Where(o => o.Pesel1 == pesel1);
                                Data = listaM;
                                NotifyPropertyChanged("Data");
                                break;
                            case "Imię":
                                listaM = model.MData.Where(o => o.FirstName1 == SearchPhrase);
                                Data = listaM;
                                NotifyPropertyChanged("Data");
                                break;
                            case "Drugie Imię":
                                listaM = model.MData.Where(o => o.MiddleName1 == SearchPhrase);
                                Data = listaM;
                                NotifyPropertyChanged("Data");
                                break;
                            case "Nazwisko":
                                listaM = model.MData.Where(o => o.LastName1 == SearchPhrase);
                                Data = listaM;
                                NotifyPropertyChanged("Data");
                                break;
                            case "Pesel2":
                                decimal pesel2 = Decimal.Parse(SearchPhrase);
                                listaM = model.MData.Where(o => o.Pesel2 == pesel2);
                                Data = listaM;
                                NotifyPropertyChanged("Data");
                                break;
                            case "Imię2":
                                listaM = model.MData.Where(o => o.FirstName2 == SearchPhrase);
                                Data = listaM;
                                NotifyPropertyChanged("Data");
                                break;
                            case "Drugie Imię2":
                                listaM = model.MData.Where(o => o.MiddleName2 == SearchPhrase);
                                Data = listaM;
                                NotifyPropertyChanged("Data");
                                break;
                            case "Nazwisko2":
                                listaM = model.MData.Where(o => o.LastName2 == SearchPhrase);
                                Data = listaM;
                                NotifyPropertyChanged("Data");
                                break;
                            case "Data":
                                DateTime date = DateTime.Parse(SearchPhrase);
                                listaM = model.MData.Where(o => o.Date == date);
                                Data = listaM;
                                NotifyPropertyChanged("Data");
                                break;
                            case "Anulowano":
                                DateTime anulled = DateTime.Parse(SearchPhrase);
                                listaM = model.MData.Where(o => o.Date == anulled);
                                Data = listaM;
                                NotifyPropertyChanged("Data");
                                break;
                            case "Opis":
                                listaM = model.MData.Where(o => o.Description == SearchPhrase);
                                Data = listaM;
                                NotifyPropertyChanged("Data");
                                break;
                        }
                        break;
                    case "Zameldowanie":
                        IEnumerable<Accomodate> listaA = new ObservableCollection<Accomodate>();
                        switch (SelectedColumn)
                        {
                            case "Pesel":
                                decimal pesel = Decimal.Parse(SearchPhrase);
                                listaA = model.AData.Where(o => o.Pesel == pesel);
                                Data = listaA;
                                NotifyPropertyChanged("Data");
                                break;
                            case "Imię":
                                listaA = model.AData.Where(o => o.FirstName == SearchPhrase);
                                Data = listaA;
                                NotifyPropertyChanged("Data");
                                break;
                            case "Nazwisko":
                                listaA = model.AData.Where(o => o.LastName == SearchPhrase);
                                Data = listaA;
                                NotifyPropertyChanged("Data");
                                break;
                            case "Kraj":
                                listaA = model.AData.Where(o => o.Country == SearchPhrase);
                                Data = listaA;
                                NotifyPropertyChanged("Data");
                                break;
                            case "Miasto":
                                listaA = model.AData.Where(o => o.City == SearchPhrase);
                                Data = listaA;
                                NotifyPropertyChanged("Data");
                                break;
                            case "Kod Pocztowy":
                                listaA = model.AData.Where(o => o.PostCode == SearchPhrase);
                                Data = listaA;
                                NotifyPropertyChanged("Data");
                                break;
                            case "Ulica":
                                listaA = model.AData.Where(o => o.Street == SearchPhrase);
                                Data = listaA;
                                NotifyPropertyChanged("Data");
                                break;
                            case "Nr Budynku":
                                listaA = model.AData.Where(o => o.BuildingNumber == SearchPhrase);
                                Data = listaA;
                                NotifyPropertyChanged("Data");
                                break;
                            case "Nr Mieszkania":
                                int flatNR = Int16.Parse(SearchPhrase);
                                listaA = model.AData.Where(o => o.FlatNumber == flatNR);
                                Data = listaA;
                                NotifyPropertyChanged("Data");
                                break;
                            case "Kraj Tymcz.":
                                listaA = model.AData.Where(o => o.TempCountry == SearchPhrase);
                                Data = listaA;
                                NotifyPropertyChanged("Data");
                                break;
                            case "Miasto Tymcz.":
                                listaA = model.AData.Where(o => o.TempCity == SearchPhrase);
                                Data = listaA;
                                NotifyPropertyChanged("Data");
                                break;
                            case "Kod Pocztowy Tymcz.":
                                listaA = model.AData.Where(o => o.TempPostCode == SearchPhrase);
                                Data = listaA;
                                NotifyPropertyChanged("Data");
                                break;
                            case "Ulica Tymcz.":
                                listaA = model.AData.Where(o => o.TempStreet == SearchPhrase);
                                Data = listaA;
                                NotifyPropertyChanged("Data");
                                break;
                            case "Nr Budynku Tymcz.":
                                listaA = model.AData.Where(o => o.TempBuildNumber == SearchPhrase);
                                Data = listaA;
                                NotifyPropertyChanged("Data");
                                break;
                            case "Nr Mieszkania Tymcz":
                                int flatNRTmp = Int16.Parse(SearchPhrase);
                                listaA = model.AData.Where(o => o.TempFlatNumber == flatNRTmp);
                                Data = listaA;
                                NotifyPropertyChanged("Data");
                                break;
                        }
                        break;
                    case "Zgony":
                        IEnumerable<Death> listaD = new ObservableCollection<Death>();
                        switch (SelectedColumn)
                        {
                            case "Pesel":
                                decimal pesel = Decimal.Parse(SearchPhrase);
                                listaD = model.DData.Where(o => o.Pesel == pesel);
                                Data = listaD;
                                NotifyPropertyChanged("Data");
                                break;
                            case "Imię":
                                listaD = model.DData.Where(o => o.FirstName == SearchPhrase);
                                Data = listaD;
                                NotifyPropertyChanged("Data");
                                break;
                            case "Drugie Imię":
                                listaD = model.DData.Where(o => o.MiddleName == SearchPhrase);
                                Data = listaD;
                                NotifyPropertyChanged("Data");
                                break;
                            case "Nazwisko":
                                listaD = model.DData.Where(o => o.LastName == SearchPhrase);
                                Data = listaD;
                                NotifyPropertyChanged("Data");
                                break;
                            case "Data":
                                DateTime date = DateTime.Parse(SearchPhrase);
                                listaD = model.DData.Where(o => o.Date == date);
                                Data = listaD;
                                NotifyPropertyChanged("Data");
                                break;
                            case "Numer Aktu":
                                int actNR = Int16.Parse(SearchPhrase);
                                listaD = model.DData.Where(o => o.ActNumber == actNR);
                                Data = listaD;
                                NotifyPropertyChanged("Data");
                                break;
                        }
                        break;
                }
            }
            else
            {
                MessageBox.Show("Wybierz kolumnę lub frazę do wyszukania!", "Błąd!", MessageBoxButton.OK, MessageBoxImage.Error);
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
