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
        public ObservableCollection<Birth> BData { get; set; }
        public ObservableCollection<Death> DData { get; set; }
        public ObservableCollection<Marriage> MData { get; set; }
        public ObservableCollection<PersonDetails> PData { get; set; }
        public ObservableCollection<Accomodate> AData { get; set; }
        public void ShowBirth()
        {
            var context = new Entities();
            var data = from p in context.Person
                       join b in context.Births on p.pesel equals b.pesel
                       select new Birth
                       {
                           Pesel = p.pesel,
                           FirstName = p.firstName,
                           MiddleName = p.middleName,
                           LastName = p.lastName,
                           Date = b.date,
                           MothersPesel = b.mothersPesel
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
                           FirstName = p.firstName,
                           MiddleName = p.middleName,
                           LastName = p.lastName,
                           Date = d.date,
                           ActNumber = d.ID
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
                           FirstName1 = p.firstName,
                           MiddleName1 = p.middleName,
                           LastName1 = p.lastName,
                           Pesel2 = m.pesel2,
                           FirstName2 = p1.firstName,
                           MiddleName2 = p1.middleName,
                           LastName2 = p1.lastName,
                           Date = m.date,
                           Anulled = m.anulled,
                           Description = m.description
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
                           FirstName = p.firstName,
                           MiddleName = p.middleName,
                           LastName = p.lastName,
                           DateOfBirth = p.dateOfBirth,
                           IsMale = p.sex
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
                           FirstName = p.firstName,
                           LastName = p.lastName,
                           Country = a.country,
                           City = a.city,
                           PostCode = a.postCode,
                           Street = a.street,
                           BuildingNumber = a.buildingNumber,
                           FlatNumber = a.flatNumber,
                           TempCountry = a1.country,
                           TempCity = a1.city,
                           TempPostCode = a1.postCode,
                           TempStreet = a1.street,
                           TempBuildNumber = a1.buildingNumber,
                           TempFlatNumber = a1.flatNumber
                       };
            AData = new ObservableCollection<Accomodate>(data);
        }      

        public void ModifyValue()
        {

        }

        public void DeleteValue()
        {

        }
    }
}

public class Birth
{
    [Display(Name="Pesel")]
    public decimal Pesel { get; set; }

    [Display(Name="Imię")]
    public string FirstName { get; set; }

    [Display(Name="DrugieImię")]
    public string MiddleName { get; set; }

    [Display(Name="Nazwisko")]
    public string LastName { get; set; }

    [Display(Name="DataUrodzenia")]
    public DateTime Date { get; set; }

    [Display(Name="Pesel Matki")]
    public decimal MothersPesel { get; set; }
}

public class Marriage
{
    [Display(Name="Pesel")]
    public decimal Pesel1 { get; set; }

    [Display(Name="Imię")]
    public string FirstName1 { get; set; }

    [Display(Name="Drugie Imię")]
    public string MiddleName1 { get; set; }

    [Display(Name="Nazwisko")]
    public string LastName1 { get; set; }

    [Display(Name="Pesel Małżonka")]
    public decimal Pesel2 { get; set; }

    [Display(Name="Imię Małżonka")]
    public string FirstName2 { get; set; }

    [Display(Name="Drugie Imię Małż.")]
    public string MiddleName2 { get; set; }

    [Display(Name="Nazwisko Małżonka")]
    public string LastName2 { get; set; }

    [Display(Name="Data zawarcia")]
    public DateTime Date { get; set; }

    [Display(Name="Anulowano")]
    public DateTime? Anulled { get; set; }

    [Display(Name="Powód")]
    public string Description { get; set; }
}

public class Death
{
    public decimal Pesel { get; set; }

    [Display(Name="Imię")]
    public string FirstName { get; set; }

    [Display(Name="Drugie Imię")]
    public string MiddleName { get; set; }

    [Display(Name="Nazwisko")]
    public string LastName { get; set; }

    [Display(Name="Data Zgonu")]
    public DateTime Date { get; set; }

    [Display(Name="Numer Aktu")]
    public int ActNumber { get; set; }
}

public class PersonDetails
{
    public decimal Pesel { get; set; }

    [Display(Name="Imię")]
    public string FirstName { get; set; }

    [Display(Name="Drugie Imię")]
    public string MiddleName { get; set; }

    [Display(Name="Nazwisko")]
    public string LastName { get; set; }

    [Display(Name="Data Urodzenia")]
    public DateTime DateOfBirth { get; set; }

    [Display(Name="CzyMężczyzna")]
    public bool IsMale { get; set; }
}

public class Accomodate
{
    public decimal Pesel { get; set; }

    [Display(Name="Imię")]
    public string FirstName { get; set; }

    [Display(Name="Nazwisko")]
    public string LastName { get; set; }

    [Display(Name="Kraj")]
    public string Country { get; set; }

    [Display(Name="Miasto")]
    public string City { get; set; }

    [Display(Name="Kod Pocztowy")]
    public string PostCode { get; set; }

    [Display(Name="Ulica")]
    public string Street { get; set; }

    [Display(Name="Numer Budynku")]
    public string BuildingNumber { get; set; }

    [Display(Name="Numer Mieszkania")]
    public int FlatNumber { get; set; }

    [Display(Name="Tymcz. Kraj")]
    public string TempCountry { get; set; }

    [Display(Name="Tymcz. Miasto")]
    public string TempCity { get; set; }

    [Display(Name="Tymcz. Kod Poczt.")]
    public string TempPostCode { get; set; }

    [Display(Name="Tymcz. Ulica")]
    public string TempStreet { get; set; }

    [Display(Name="Tymcz. Nr Budynku")]
    public string TempBuildNumber { get; set; }

    [Display(Name="Tymcz. Nr Mieszkania")]
    public int TempFlatNumber { get; set; }
}