//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ProjInzynierski
{
    using System;
    using System.Collections.ObjectModel;
    
    public partial class Deaths
    {
        public decimal pesel { get; set; }
        public int ID { get; set; }
        public System.DateTime date { get; set; }
    
        public virtual Person Person { get; set; }
    }
}
