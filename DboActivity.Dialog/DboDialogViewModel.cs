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
                            if (res.HasValue && res.Value && insertVM.FirstName != null && insertVM.LastName != null && insertVM.MothersPesel!=null)
                            {
                                context.Person.Add(FillPerson(insertVM.Pesel, insertVM.FirstName, insertVM.MiddleName, insertVM.LastName, insertVM.DateOfBirth, insertVM.Sex));
                                context.Births.Add(FillBirth(1, insertVM.Date, insertVM.Pesel, insertVM.MothersPesel));
                                context.SaveChanges();
                                Birth birth = new Birth();
                                birth.Pesel = insertVM.Pesel; birth.Imie = insertVM.FirstName; birth.DrugieImie = insertVM.MiddleName;
                                birth.Nazwisko = insertVM.LastName; birth.Data = insertVM.DateOfBirth; birth.PeselMatki = insertVM.MothersPesel;
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
                            if (res.HasValue && res.Value && insertVM.FirstName!=null && insertVM.LastName!=null)
                            {
                                context.Deaths.Add(FillDeath(1, insertVM.Date, insertVM.Pesel));
                                var personList = context.Person.ToList();
                                Person personToUpdate = personList.Where(p => p.pesel.Equals(insertVM.Pesel)).FirstOrDefault<Person>();
                                personToUpdate.isDead = true;
                                context.SaveChanges();
                                Death death = new Death();
                                death.Imie = insertVM.FirstName; death.Nazwisko = insertVM.LastName; death.DrugieImie = insertVM.MiddleName;
                                death.Pesel = insertVM.Pesel; death.Data = insertVM.Date;
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
                            if (res.HasValue && res.Value && addVM.FirstName1 != null && addVM.LastName1 != null && addVM.FirstName2 != null && addVM.LastName2 != null)
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
                                marriage.Pesel1 = addVM.Pesel1; marriage.Imie1 = addVM.FirstName1; marriage.DrugieImie1 = addVM.MiddleName1;
                                marriage.Nazwisko1 = addVM.LastName1; marriage.Pesel2 = addVM.Pesel2; marriage.Imie2 = addVM.FirstName2;
                                marriage.DrugieImie2 = addVM.MiddleName2; marriage.Nazwisko2 = addVM.LastName2; marriage.Data = addVM.Date;
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
                            if (res.HasValue && res.Value && insertVM.FirstName!=null && insertVM.LastName!=null)
                            {
                                context.Person.Add(FillPerson(insertVM.Pesel, insertVM.FirstName, insertVM.MiddleName, insertVM.LastName, insertVM.DateOfBirth, insertVM.Sex));
                                context.SaveChanges();
                                PersonDetails persona = new PersonDetails();
                                persona.Pesel = insertVM.Pesel; persona.Imie = insertVM.FirstName; persona.DrugieImie = insertVM.MiddleName;
                                persona.Nazwisko = insertVM.LastName; persona.CzyMężczyzna = insertVM.Sex; persona.DataUrodzenia = insertVM.DateOfBirth;
                                model.PData.Add(persona);
                                NotifyPropertyChanged("Data");
                            }
                            else
                            {
                                MessageBox.Show("Podaj wszystkie dane!", "Błąd!",MessageBoxButton.OK,MessageBoxImage.Error);
                            }                          
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
                            modVM.Pesel = birth.Pesel; modVM.MothersPesel = birth.PeselMatki; modVM.DateOfBirth = birth.Data;
                            modVM.FirstName = birth.Imie; modVM.MiddleName = birth.DrugieImie; modVM.LastName = birth.Nazwisko;
                            Modify_Dialog_b dialog = new Modify_Dialog_b(modVM);
                            var birthList = context.Births.ToList();
                            Births birthToMod = birthList.Where(o => o.pesel.Equals(modVM.Pesel)).FirstOrDefault<Births>();
                            bool? res = dialog.ShowDialog();
                            if (res.HasValue && res.Value)
                            {
                                
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
                            modVM.Pesel = death.Pesel; modVM.Code = death.NumerAktu; modVM.DateOfBirth = death.Data;
                            modVM.FirstName = death.Imie; modVM.MiddleName = death.DrugieImie; modVM.LastName = death.Nazwisko;
                            ModifyDialog_d dialog = new ModifyDialog_d(modVM);
                            var deathList = context.Deaths.ToList();
                            Deaths deathToMod = deathList.Where(o => o.pesel.Equals(modVM.Pesel)).FirstOrDefault<Deaths>();
                            bool? res = dialog.ShowDialog();
                            if (res.HasValue && res.Value)
                            {
                                
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
                            modVM.Pesel = acc.Pesel; modVM.FirstName = acc.Imie; modVM.LastName = acc.Nazwisko;
                            ModifyDialog_a dialog = new ModifyDialog_a(modVM);
                            var accList = context.Person.ToList();
                            Person accToMod = accList.Where(o => o.pesel.Equals(modVM.Pesel)).FirstOrDefault<Person>();
                            bool? res = dialog.ShowDialog();                           
                            if (res.HasValue && res.Value)
                            {
                                
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
                            modVM.Pesel = personD.Pesel; modVM.DateOfBirth = personD.DataUrodzenia; personD.CzyMężczyzna = modVM.Sex;
                            modVM.FirstName = personD.Imie; modVM.MiddleName = personD.DrugieImie; modVM.LastName = personD.Nazwisko;
                            ModifyDialogWindow_p dialog = new ModifyDialogWindow_p(modVM);
                            var personList = context.Person.ToList();
                            Person personToMod = personList.Where(o => o.pesel.Equals(modVM.Pesel)).FirstOrDefault<Person>();
                            bool? res = dialog.ShowDialog();
                            if (res.HasValue && res.Value)
                            {                                
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
                            modVM.Pesel = marriage.Pesel1; modVM.Date = marriage.Data; modVM.Pesel1 = marriage.Pesel2; modVM.Anulled = marriage.Anulowano;
                            modVM.FirstName = marriage.Imie1; modVM.MiddleName = marriage.DrugieImie1; modVM.LastName = marriage.Nazwisko1;
                            modVM.FirstName1 = marriage.Imie2; modVM.MiddleName1 = marriage.DrugieImie2; modVM.LastName1 = marriage.Nazwisko2;
                            modVM.Description = marriage.Powod;
                            ModifyDialog_m dialog = new ModifyDialog_m(modVM);
                            var marriageList = context.Marriages.ToList();
                            Marriages marriageToMod = marriageList.Where(o => o.pesel.Equals(modVM.Pesel)).FirstOrDefault<Marriages>();
                            bool? res = dialog.ShowDialog();
                            if (res.HasValue && res.Value)
                            {
                                
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
            if (combo != null && SearchPhrase != null && SearchPhrase != string.Empty)
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
                                listaP = model.PData.Where(o => o.Imie == SearchPhrase);
                                Data = listaP;
                                NotifyPropertyChanged("Data");
                                break;
                            case "Drugie Imię":
                                listaP = model.PData.Where(o => o.DrugieImie == SearchPhrase);
                                Data = listaP;
                                NotifyPropertyChanged("Data");
                                break;
                            case "Nazwisko":
                                listaP = model.PData.Where(o => o.Nazwisko == SearchPhrase);
                                Data = listaP;
                                NotifyPropertyChanged("Data");
                                break;
                            case "Data Urodzenia":
                                DateTime date = DateTime.Parse(SearchPhrase);
                                listaP = model.PData.Where(o => o.DataUrodzenia == date);
                                Data = listaP;
                                NotifyPropertyChanged("Data");
                                break;
                            case "Czy Mężczyzna":
                                bool ismale = Boolean.Parse(SearchPhrase);
                                listaP = model.PData.Where(o => o.CzyMężczyzna == ismale);
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
                                listaB = model.BData.Where(o => o.Imie == SearchPhrase);
                                Data = listaB;
                                NotifyPropertyChanged("Data");
                                break;
                            case "Drugie Imię":
                                listaB = model.BData.Where(o => o.DrugieImie == SearchPhrase);
                                Data = listaB;
                                NotifyPropertyChanged("Data");
                                break;
                            case "Nazwisko":
                                listaB = model.BData.Where(o => o.Nazwisko == SearchPhrase);
                                Data = listaB;
                                NotifyPropertyChanged("Data");
                                break;
                            case "Data":
                                DateTime date = DateTime.Parse(SearchPhrase);
                                listaB = model.BData.Where(o => o.Data == date);
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
                                listaM = model.MData.Where(o => o.Imie1 == SearchPhrase);
                                Data = listaM;
                                NotifyPropertyChanged("Data");
                                break;
                            case "Drugie Imię":
                                listaM = model.MData.Where(o => o.DrugieImie1 == SearchPhrase);
                                Data = listaM;
                                NotifyPropertyChanged("Data");
                                break;
                            case "Nazwisko":
                                listaM = model.MData.Where(o => o.Nazwisko1 == SearchPhrase);
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
                                listaM = model.MData.Where(o => o.Imie2 == SearchPhrase);
                                Data = listaM;
                                NotifyPropertyChanged("Data");
                                break;
                            case "Drugie Imię2":
                                listaM = model.MData.Where(o => o.DrugieImie2 == SearchPhrase);
                                Data = listaM;
                                NotifyPropertyChanged("Data");
                                break;
                            case "Nazwisko2":
                                listaM = model.MData.Where(o => o.Nazwisko2 == SearchPhrase);
                                Data = listaM;
                                NotifyPropertyChanged("Data");
                                break;
                            case "Data":
                                DateTime date = DateTime.Parse(SearchPhrase);
                                listaM = model.MData.Where(o => o.Data == date);
                                Data = listaM;
                                NotifyPropertyChanged("Data");
                                break;
                            case "Anulowano":
                                DateTime anulled = DateTime.Parse(SearchPhrase);
                                listaM = model.MData.Where(o => o.Data == anulled);
                                Data = listaM;
                                NotifyPropertyChanged("Data");
                                break;
                            case "Opis":
                                listaM = model.MData.Where(o => o.Powod == SearchPhrase);
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
                                listaA = model.AData.Where(o => o.Imie == SearchPhrase);
                                Data = listaA;
                                NotifyPropertyChanged("Data");
                                break;
                            case "Nazwisko":
                                listaA = model.AData.Where(o => o.Nazwisko == SearchPhrase);
                                Data = listaA;
                                NotifyPropertyChanged("Data");
                                break;
                            case "Kraj":
                                listaA = model.AData.Where(o => o.Kraj == SearchPhrase);
                                Data = listaA;
                                NotifyPropertyChanged("Data");
                                break;
                            case "Miasto":
                                listaA = model.AData.Where(o => o.Miasto == SearchPhrase);
                                Data = listaA;
                                NotifyPropertyChanged("Data");
                                break;
                            case "Kod Pocztowy":
                                listaA = model.AData.Where(o => o.KodPocztowy == SearchPhrase);
                                Data = listaA;
                                NotifyPropertyChanged("Data");
                                break;
                            case "Ulica":
                                listaA = model.AData.Where(o => o.Ulica == SearchPhrase);
                                Data = listaA;
                                NotifyPropertyChanged("Data");
                                break;
                            case "Nr Budynku":
                                listaA = model.AData.Where(o => o.NrBudynku == SearchPhrase);
                                Data = listaA;
                                NotifyPropertyChanged("Data");
                                break;
                            case "Nr Mieszkania":
                                int flatNR = Int16.Parse(SearchPhrase);
                                listaA = model.AData.Where(o => o.NrMieszkania == flatNR);
                                Data = listaA;
                                NotifyPropertyChanged("Data");
                                break;
                            case "Kraj Tymcz.":
                                listaA = model.AData.Where(o => o.TymczKraj == SearchPhrase);
                                Data = listaA;
                                NotifyPropertyChanged("Data");
                                break;
                            case "Miasto Tymcz.":
                                listaA = model.AData.Where(o => o.TymczMiasto == SearchPhrase);
                                Data = listaA;
                                NotifyPropertyChanged("Data");
                                break;
                            case "Kod Pocztowy Tymcz.":
                                listaA = model.AData.Where(o => o.TymczKodPoczt == SearchPhrase);
                                Data = listaA;
                                NotifyPropertyChanged("Data");
                                break;
                            case "Ulica Tymcz.":
                                listaA = model.AData.Where(o => o.TymczUlica == SearchPhrase);
                                Data = listaA;
                                NotifyPropertyChanged("Data");
                                break;
                            case "Nr Budynku Tymcz.":
                                listaA = model.AData.Where(o => o.TymczNrBudynku == SearchPhrase);
                                Data = listaA;
                                NotifyPropertyChanged("Data");
                                break;
                            case "Nr Mieszkania Tymcz":
                                int flatNRTmp = Int16.Parse(SearchPhrase);
                                listaA = model.AData.Where(o => o.TymczNrMieszkania == flatNRTmp);
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
                                listaD = model.DData.Where(o => o.Imie == SearchPhrase);
                                Data = listaD;
                                NotifyPropertyChanged("Data");
                                break;
                            case "Drugie Imię":
                                listaD = model.DData.Where(o => o.DrugieImie == SearchPhrase);
                                Data = listaD;
                                NotifyPropertyChanged("Data");
                                break;
                            case "Nazwisko":
                                listaD = model.DData.Where(o => o.Nazwisko == SearchPhrase);
                                Data = listaD;
                                NotifyPropertyChanged("Data");
                                break;
                            case "Data":
                                DateTime date = DateTime.Parse(SearchPhrase);
                                listaD = model.DData.Where(o => o.Data == date);
                                Data = listaD;
                                NotifyPropertyChanged("Data");
                                break;
                            case "Numer Aktu":
                                int actNR = Int16.Parse(SearchPhrase);
                                listaD = model.DData.Where(o => o.NumerAktu == actNR);
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
