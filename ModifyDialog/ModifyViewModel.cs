using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModifyDialog
{
    public class ModifyViewModel
    {
        public decimal Pesel { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime Date { get; set; }
        public int Code { get; set; }
        public int IDTemp { get; set; }
        public int IDPerm { get; set; }
        public bool Sex { get; set; }
        public DateTime? Anulled { get; set; }
        public string Description { get; set; }
        public bool IsDead { get; set; }
        public decimal MothersPesel { get; set; }
        public decimal Pesel1 { get; set; }
        public string FirstName1 { get; set; }
        public string MiddleName1 { get; set; }
        public string LastName1 { get; set; }
    }
}
