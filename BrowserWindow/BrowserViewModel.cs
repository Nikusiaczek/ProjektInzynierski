﻿using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Data.SqlClient;
using DboActivity.Dialog;
using System.Windows;
using System.Collections.ObjectModel;

namespace BrowserWindow
{
    public class BrowserViewModel : INotifyPropertyChanged
    {
        private readonly DelegateCommand<object> _browse;
        private readonly DelegateCommand<object> _pickTable;
        Model model = new Model();
        private object _data;
        private List<string> _tables = new List<string>();
        private string connectionString = "Data Source=AGALAP;Initial Catalog=\"D:\\PROJEKTY VISUAL\\POPULATIONREGISTERING\\POPULATIONREGISTER.MDF\";Integrated Security=True";

        public BrowserViewModel()
        {
            _browse = new DelegateCommand<object>(Browse);
            _pickTable = new DelegateCommand<object>(PickTable);
            _tables.Add("Dane Osobowe");
            _tables.Add("Małżeństwa");
            _tables.Add("Narodziny");
            _tables.Add("Zameldowanie");
            _tables.Add("Zgony");
        }

        public ICommand BrowseCommand { get { return _browse; } }
        public ICommand PickTableCommand { get { return _pickTable; } }

        public string TableChoice { get; set; }
        public object Data
        {
            get { return _data; }
            set { _data = value; NotifyPropertyChanged("Data"); }
        }
        public string SearchPhrase { get; set; }
        public string SelectedColumn { get; set; }
        public object ColumnSet
        {
            get { return _data; }
            set { _data = value; NotifyPropertyChanged("ColumnSet"); }
        }
        public List<string> Tables
        {
            get { return _tables; }
        }

        public void Browse(object sender)
        {
            ComboBox combo = sender as ComboBox;
            if (TableChoice != null && SearchPhrase != null && combo != null)
            {
                SelectedColumn = combo.SelectedValue.ToString();
                switch (TableChoice)
                {
                    case "Dane Osobowe":
                        IEnumerable<PersonDetails> listaP = new ObservableCollection<PersonDetails>();
                        switch (SelectedColumn)
                        {
                            case "Pesel":
                                decimal pesel = Decimal.Parse(SearchPhrase);
                                listaP = model.PData.Where(o => o.Pesel == pesel);
                                Data = listaP;
                                break;
                            case "Imię":
                                listaP = model.PData.Where(o => o.Imie == SearchPhrase);
                                Data = listaP;
                                break;
                            case "Drugie Imię":
                                listaP = model.PData.Where(o => o.DrugieImie == SearchPhrase);
                                Data = listaP;
                                break;
                            case "Nazwisko":
                                listaP = model.PData.Where(o => o.Nazwisko == SearchPhrase);
                                Data = listaP;
                                break;
                            case "Data Urodzenia":
                                DateTime date = DateTime.Parse(SearchPhrase);
                                listaP = model.PData.Where(o => o.DataUrodzenia == date);
                                Data = listaP;
                                break;
                            case "Czy Mężczyzna":
                                bool ismale = Boolean.Parse(SearchPhrase);
                                listaP = model.PData.Where(o => o.CzyMężczyzna == ismale);
                                Data = listaP;
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
                                break;
                            case "Imię":
                                listaM = model.MData.Where(o => o.Imie1 == SearchPhrase);
                                Data = listaM;
                                break;
                            case "Drugie Imię":
                                listaM = model.MData.Where(o => o.DrugieImie1 == SearchPhrase);
                                Data = listaM;
                                break;
                            case "Nazwisko":
                                listaM = model.MData.Where(o => o.Nazwisko1 == SearchPhrase);
                                Data = listaM;
                                break;
                            case "Pesel2":
                                decimal pesel2 = Decimal.Parse(SearchPhrase);
                                listaM = model.MData.Where(o => o.Pesel2 == pesel2);
                                Data = listaM;
                                break;
                            case "Imię2":
                                listaM = model.MData.Where(o => o.Imie2 == SearchPhrase);
                                Data = listaM;
                                break;
                            case "Drugie Imię2":
                                listaM = model.MData.Where(o => o.DrugieImie2 == SearchPhrase);
                                Data = listaM;
                                break;
                            case "Nazwisko2":
                                listaM = model.MData.Where(o => o.Nazwisko2 == SearchPhrase);
                                Data = listaM;
                                break;
                            case "Data":
                                DateTime date = DateTime.Parse(SearchPhrase);
                                listaM = model.MData.Where(o => o.Data == date);
                                Data = listaM;
                                break;
                            case "Anulowano":
                                DateTime anulled = DateTime.Parse(SearchPhrase);
                                listaM = model.MData.Where(o => o.Data == anulled);
                                Data = listaM;
                                break;
                            case "Opis":
                                listaM = model.MData.Where(o => o.Powod == SearchPhrase);
                                Data = listaM;
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
                                break;
                            case "Imię":
                                listaB = model.BData.Where(o => o.Imie == SearchPhrase);
                                Data = listaB;
                                break;
                            case "Drugie Imię":
                                listaB = model.BData.Where(o => o.DrugieImie == SearchPhrase);
                                Data = listaB;
                                break;
                            case "Nazwisko":
                                listaB = model.BData.Where(o => o.Nazwisko == SearchPhrase);
                                Data = listaB;
                                break;
                            case "Data":
                                DateTime date = DateTime.Parse(SearchPhrase);
                                listaB = model.BData.Where(o => o.Data == date);
                                Data = listaB;
                                break;
                            case "Pesel Matki":
                                decimal mompesel = Decimal.Parse(SearchPhrase);
                                listaB = model.BData.Where(o => o.Pesel == mompesel);
                                Data = listaB;
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
                                break;
                            case "Imię":
                                listaA = model.AData.Where(o => o.Imie == SearchPhrase);
                                Data = listaA;
                                break;
                            case "Nazwisko":
                                listaA = model.AData.Where(o => o.Nazwisko == SearchPhrase);
                                Data = listaA;
                                break;
                            case "Kraj":
                                listaA = model.AData.Where(o => o.Kraj == SearchPhrase);
                                Data = listaA;
                                break;
                            case "Miasto":
                                listaA = model.AData.Where(o => o.Miasto == SearchPhrase);
                                Data = listaA;
                                break;
                            case "Kod Pocztowy":
                                listaA = model.AData.Where(o => o.KodPocztowy == SearchPhrase);
                                Data = listaA;
                                break;
                            case "Ulica":
                                listaA = model.AData.Where(o => o.Ulica == SearchPhrase);
                                Data = listaA;
                                break;
                            case "Nr Budynku":
                                listaA = model.AData.Where(o => o.NrBudynku == SearchPhrase);
                                Data = listaA;
                                break;
                            case "Nr Mieszkania":
                                int flatNR = Int16.Parse(SearchPhrase);
                                listaA = model.AData.Where(o => o.NrMieszkania == flatNR);
                                Data = listaA;
                                break;
                            case "Kraj Tymcz.":
                                listaA = model.AData.Where(o => o.TymczKraj == SearchPhrase);
                                Data = listaA;
                                break;
                            case "Miasto Tymcz.":
                                listaA = model.AData.Where(o => o.TymczMiasto == SearchPhrase);
                                Data = listaA;
                                break;
                            case "Kod Pocztowy Tymcz.":
                                listaA = model.AData.Where(o => o.TymczKodPoczt == SearchPhrase);
                                Data = listaA;
                                break;
                            case "Ulica Tymcz.":
                                listaA = model.AData.Where(o => o.TymczUlica == SearchPhrase);
                                Data = listaA;
                                break;
                            case "Nr Budynku Tymcz.":
                                listaA = model.AData.Where(o => o.TymczNrBudynku == SearchPhrase);
                                Data = listaA;
                                break;
                            case "Nr Mieszkania Tymcz":
                                int flatNRTmp = Int16.Parse(SearchPhrase);
                                listaA = model.AData.Where(o => o.TymczNrMieszkania == flatNRTmp);
                                Data = listaA;
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
                                break;
                            case "Imię":
                                listaD = model.DData.Where(o => o.Imie == SearchPhrase);
                                Data = listaD;
                                break;
                            case "Drugie Imię":
                                listaD = model.DData.Where(o => o.DrugieImie == SearchPhrase);
                                Data = listaD;
                                break;
                            case "Nazwisko":
                                listaD = model.DData.Where(o => o.Nazwisko == SearchPhrase);
                                Data = listaD;
                                break;
                            case "Data":
                                DateTime date = DateTime.Parse(SearchPhrase);
                                listaD = model.DData.Where(o => o.Data == date);
                                Data = listaD;
                                break;
                            case "Numer Aktu":
                                int actNR = Int16.Parse(SearchPhrase);
                                listaD = model.DData.Where(o => o.NumerAktu == actNR);
                                Data = listaD;
                                break;
                        }
                        break;
                }
            }
            else if (TableChoice == null)
            {
                MessageBox.Show("Wybierz dział!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                MessageBox.Show("Wybierz kolumnę lub wpisz frazę!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PickTable(object sender)
        {
            ComboBox combo = sender as ComboBox;
            if (combo != null)
            {
                TableChoice = combo.SelectedValue.ToString();
                using (SqlConnection db = new SqlConnection(connectionString))
                {
                    db.Open();
                    switch (TableChoice)
                    {
                        case "Dane Osobowe":
                            model.ShowPersonalData();
                            Data = model.PData;
                            model.CreateColumnSet(TableChoice);
                            ColumnSet = model.ColumnSet;
                            break;
                        case "Małżeństwa":
                            model.ShowMarriage();
                            Data = model.MData;
                            model.CreateColumnSet(TableChoice);
                            ColumnSet = model.ColumnSet;
                            break;
                        case "Narodziny":
                            model.ShowBirth();
                            Data = model.BData;
                            model.CreateColumnSet(TableChoice);
                            ColumnSet = model.ColumnSet;
                            break;
                        case "Zameldowanie":
                            model.ShowAccomodation();
                            Data = model.AData;
                            model.CreateColumnSet(TableChoice);
                            ColumnSet = model.ColumnSet;
                            break;
                        case "Zgony":
                            model.ShowDeath();
                            Data = model.DData;
                            model.CreateColumnSet(TableChoice);
                            ColumnSet = model.ColumnSet;
                            break;
                    }
                    db.Close();
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
