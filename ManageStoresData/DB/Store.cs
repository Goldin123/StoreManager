//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ManageStoresData.DB
{
    using System;
    using System.Collections.Generic;
    
    public partial class Store
    {
        public int SID { get; set; }
        public Nullable<int> ID { get; set; }
        public string StoreName { get; set; }
        public string TelephoneNumber { get; set; }
        public Nullable<System.DateTime> OpenDate { get; set; }
        public Nullable<System.DateTime> DateAdded { get; set; }
        public Nullable<System.DateTime> DateUpdated { get; set; }
    }
}
