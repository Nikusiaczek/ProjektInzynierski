//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DboActivity.Dialog
{
    using System;
    using System.Collections.ObjectModel;
    
    public partial class Marriages
    {
        public int ID { get; set; }
        public decimal pesel { get; set; }
        public decimal pesel2 { get; set; }
        public System.DateTime date { get; set; }
        public Nullable<System.DateTime> anulled { get; set; }
        public string description { get; set; }
    
        public virtual Person Person { get; set; }
        public virtual Person Person1 { get; set; }
    }
}
