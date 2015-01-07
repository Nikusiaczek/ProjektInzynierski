using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DboActivity.Dialog;
using System.ComponentModel.DataAnnotations;

namespace DboActivity.Dialog
{
    public class Model
    {
        private List<string> _columnSet = new List<string>();
        public ObservableCollection<Birth> BData { get; set; }
        public ObservableCollection<Death> DData { get; set; }
        public ObservableCollection<Marriage> MData { get; set; }
        public ObservableCollection<PersonDetails> PData { get; set; }
        public ObservableCollection<Accomodate> AData { get; set; }
        public List<string> ColumnSet { get { return _columnSet; } }

        public void CreateColumnSet(string tableName)
        {
            _columnSet.Clear();
            switch (tableName)
            {
                case "Dane Osobowe":
                    _columnSet.Add("Pesel");
                    _columnSet.Add("Imię");
                    _columnSet.Add("Drugie Imię");
                    _columnSet.Add("Nazwisko");
                    _columnSet.Add("Data Urodzenia");
                    _columnSet.Add("Czy Mężczyzna");
                    break;
                case "Małżeństwa":
                    _columnSet.Add("Pesel1");
                    _columnSet.Add("Imię");
                    _columnSet.Add("Drugie Imię");
                    _columnSet.Add("Nazwisko");
                    _columnSet.Add("Pesel2");
                    _columnSet.Add("Imię2");
                    _columnSet.Add("Drugie Imię2");
                    _columnSet.Add("Nazwisko2");
                    _columnSet.Add("Data");
                    _columnSet.Add("Anulowano");
                    _columnSet.Add("Opis");
                    break;
                case "Narodziny":
                    _columnSet.Add("Pesel");
                    _columnSet.Add("Imię");
                    _columnSet.Add("Drugie Imię");
                    _columnSet.Add("Nazwisko");
                    _columnSet.Add("Data");
                    _columnSet.Add("Pesel Matki");
                    break;
                case "Zameldowanie":
                    _columnSet.Add("Pesel");
                    _columnSet.Add("Imię");
                    _columnSet.Add("Nazwisko");
                    _columnSet.Add("Kraj");
                    _columnSet.Add("Miasto");
                    _columnSet.Add("Kod Pocztowy");
                    _columnSet.Add("Ulica");
                    _columnSet.Add("Nr Budynku");
                    _columnSet.Add("Nr Mieszkania");
                    _columnSet.Add("Kraj Tymcz.");
                    _columnSet.Add("Miasto Tymcz.");
                    _columnSet.Add("Kod Pocztowy Tymcz.");
                    _columnSet.Add("Ulica Tymcz.");
                    _columnSet.Add("Nr Budynku Tymcz.");
                    _columnSet.Add("Nr Mieszkania Tymcz");
                    break;
                case "Zgony":
                    _columnSet.Add("Pesel");
                    _columnSet.Add("Imię");
                    _columnSet.Add("Drugie Imię");
                    _columnSet.Add("Nazwisko");
                    _columnSet.Add("Data");
                    _columnSet.Add("Numer Aktu");
                    break;
            }
        }

        public void ShowBirth()
        {
            var context = new Entities();
            var data = from p in context.Person
                       join b in context.Births on p.pesel equals b.pesel
                       select new Birth
                       {
                           Pesel = p.pesel,
                           Imie = p.firstName,
                           DrugieImie = p.middleName,
                           Nazwisko = p.lastName,
                           Data = b.date,
                           PeselMatki = b.mothersPesel
                       };
            BData = new ObservableCollection<Birth>(data);
        }

        public void ShowDeath()
        {
            var context = new Entities();
            var data = from p in context.Person
                       join d in context.Deaths on p.pesel equals d.pesel
                       select new Death
                       {
                           Pesel = p.pesel,
                           Imie = p.firstName,
                           DrugieImie = p.middleName,
                           Nazwisko = p.lastName,
                           Data = d.date,
                           NumerAktu = d.ID
                       };
            DData = new ObservableCollection<Death>(data);
        }

        public void ShowMarriage()
        {
            var context = new Entities();
            var data = from m in context.Marriages
                       join p in context.Person on m.pesel equals p.pesel
                       join p1 in context.Person on m.pesel2 equals p1.pesel
                       select new Marriage
                       {
                           Pesel1 = m.pesel,
                           Imie1 = p.firstName,
                           DrugieImie1 = p.middleName,
                           Nazwisko1 = p.lastName,
                           Pesel2 = m.pesel2,
                           Imie2 = p1.firstName,
                           DrugieImie2 = p1.middleName,
                           Nazwisko2 = p1.lastName,
                           Data = m.date,
                           Anulowano = m.anulled,
                           Powod = m.description
                       };
            MData = new ObservableCollection<Marriage>(data);
        }

        public void ShowPersonalData()
        {
            var context = new Entities();
            var data = from p in context.Person
                       where !(p.isDead.HasValue && p.isDead.Value.Equals(true))
                       select new PersonDetails
                       {
                           Pesel = p.pesel,
                           Imie = p.firstName,
                           DrugieImie = p.middleName,
                           Nazwisko = p.lastName,
                           DataUrodzenia = p.dateOfBirth,
                           CzyMężczyzna = p.sex
                       };
            PData = new ObservableCollection<PersonDetails>(data);
        }

        public void ShowAccomodation()
        {
            var context = new Entities();
            var data = from p in context.Person
                       join a in context.Accommodation on p.permanentAddress_ID equals a.ID
                       join a1 in context.Accommodation on p.temporaryAddress_ID equals a1.ID
                       select new Accomodate
                       {
                           Pesel = p.pesel,
                           Imie = p.firstName,
                           Nazwisko = p.lastName,
                           Kraj = a.country,
                           Miasto = a.city,
                           KodPocztowy = a.postCode,
                           Ulica = a.street,
                           NrBudynku = a.buildingNumber,
                           NrMieszkania = a.flatNumber,
                           TymczKraj = a1.country,
                           TymczMiasto = a1.city,
                           TymczKodPoczt = a1.postCode,
                           TymczUlica = a1.street,
                           TymczNrBudynku = a1.buildingNumber,
                           TymczNrMieszkania = a1.flatNumber
                       };
            AData = new ObservableCollection<Accomodate>(data);
        }

        public bool ValidatePesel(decimal pesel, DateTime date, bool isMan)
        {
            string count = pesel.ToString();
            if (count.Length == 11)
                return false;
            string year = count.Substring(0, 2);
            string month = count.Substring(2, 2);
            string day = count.Substring(4, 2);
            switch (month.ElementAt(0))
            {
                case '8':
                    year = "18" + year;
                    month = month.ElementAt(1).ToString();
                    break;
                case '9':
                    year = "18" + year;
                    month = "1" + month.ElementAt(1).ToString();
                    break;
                case '0':
                    year = "19" + year;
                    month = month.ElementAt(1).ToString();
                    break;
                case '1':
                    year = "19" + year;
                    month = "1" + month.ElementAt(1).ToString();
                    break;
                case '2':
                    year = "20" + year;
                    month = month.ElementAt(1).ToString();
                    break;
                case '3':
                    year = "20" + year;
                    month = "1" + month.ElementAt(1).ToString();
                    break;
                case '4':
                    year = "21" + year;
                    month = month.ElementAt(1).ToString();
                    break;
                case '5':
                    year = "21" + year;
                    month = "1" + month.ElementAt(1).ToString();
                    break;
            }
            if (!day.Equals(date.Day) || !year.Equals(date.Year) || !month.Equals(date.Month))
                return false;
            int sex = int.Parse(count.ElementAt(9).ToString());
            if ((sex % 2 == 0 && isMan) || (sex % 2 != 0 && !isMan))
                return false;
            int controlSum = int.Parse(count.ElementAt(0).ToString()) + int.Parse(count.ElementAt(4).ToString()) + int.Parse(count.ElementAt(8).ToString());
            controlSum += (int.Parse(count.ElementAt(1).ToString()) + int.Parse(count.ElementAt(5).ToString()) + int.Parse(count.ElementAt(9).ToString())) * 3;
            controlSum += (int.Parse(count.ElementAt(2).ToString()) + int.Parse(count.ElementAt(6).ToString())) * 7;
            controlSum += (int.Parse(count.ElementAt(3).ToString()) + int.Parse(count.ElementAt(7).ToString())) * 9;
            int modulo = controlSum % 10;
            if ((10 - modulo) == int.Parse(count.ElementAt(10).ToString()))
                return true;
            return false;
        }
    }

        public class Birth
        {
            [Display(Name = "Pesel")]
            public decimal Pesel { get; set; }

            [Display(Name = "Imię")]
            public string Imie { get; set; }

            [Display(Name = "DrugieImię")]
            public string DrugieImie { get; set; }

            [Display(Name = "Nazwisko")]
            public string Nazwisko { get; set; }

            [Display(Name = "DataUrodzenia")]
            public DateTime Data { get; set; }

            [Display(Name = "Pesel Matki")]
            public decimal PeselMatki { get; set; }
        }

        public class Marriage
        {
            [Display(Name = "Pesel")]
            public decimal Pesel1 { get; set; }

            [Display(Name = "Imię")]
            public string Imie1 { get; set; }

            [Display(Name = "Drugie Imię")]
            public string DrugieImie1 { get; set; }

            [Display(Name = "Nazwisko")]
            public string Nazwisko1 { get; set; }

            [Display(Name = "Pesel Małżonka")]
            public decimal Pesel2 { get; set; }

            [Display(Name = "Imię Małżonka")]
            public string Imie2 { get; set; }

            [Display(Name = "Drugie Imię Małż.")]
            public string DrugieImie2 { get; set; }

            [Display(Name = "Nazwisko Małżonka")]
            public string Nazwisko2 { get; set; }

            [Display(Name = "Data zawarcia")]
            public DateTime Data { get; set; }

            [Display(Name = "Anulowano")]
            public DateTime? Anulowano { get; set; }

            [Display(Name = "Powód")]
            public string Powod { get; set; }
        }

        public class Death
        {
            public decimal Pesel { get; set; }

            [Display(Name = "Imię")]
            public string Imie { get; set; }

            [Display(Name = "Drugie Imię")]
            public string DrugieImie { get; set; }

            [Display(Name = "Nazwisko")]
            public string Nazwisko { get; set; }

            [Display(Name = "Data Zgonu")]
            public DateTime Data { get; set; }

            [Display(Name = "Numer Aktu")]
            public int NumerAktu { get; set; }
        }

        public class PersonDetails
        {
            public decimal Pesel { get; set; }

            [Display(Name = "Imię")]
            public string Imie { get; set; }

            [Display(Name = "Drugie Imię")]
            public string DrugieImie { get; set; }

            [Display(Name = "Nazwisko")]
            public string Nazwisko { get; set; }

            [Display(Name = "Data Urodzenia")]
            public DateTime DataUrodzenia { get; set; }

            [Display(Name = "CzyMężczyzna")]
            public bool CzyMężczyzna { get; set; }
        }

        public class Accomodate
        {
            public decimal Pesel { get; set; }

            [Display(Name = "Imię")]
            public string Imie { get; set; }

            [Display(Name = "Nazwisko")]
            public string Nazwisko { get; set; }

            [Display(Name = "Kraj")]
            public string Kraj { get; set; }

            [Display(Name = "Miasto")]
            public string Miasto { get; set; }

            [Display(Name = "Kod Pocztowy")]
            public string KodPocztowy { get; set; }

            [Display(Name = "Ulica")]
            public string Ulica { get; set; }

            [Display(Name = "Numer Budynku")]
            public string NrBudynku { get; set; }

            [Display(Name = "Numer Mieszkania")]
            public int NrMieszkania { get; set; }

            [Display(Name = "Tymcz. Kraj")]
            public string TymczKraj { get; set; }

            [Display(Name = "Tymcz. Miasto")]
            public string TymczMiasto { get; set; }

            [Display(Name = "Tymcz. Kod Poczt.")]
            public string TymczKodPoczt { get; set; }

            [Display(Name = "Tymcz. Ulica")]
            public string TymczUlica { get; set; }

            [Display(Name = "Tymcz. Nr Budynku")]
            public string TymczNrBudynku { get; set; }

            [Display(Name = "Tymcz. Nr Mieszkania")]
            public int TymczNrMieszkania { get; set; }
        }
}