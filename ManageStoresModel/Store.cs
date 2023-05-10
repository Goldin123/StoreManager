using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageStoresModel
{
    public class StoreDetail
    {
        public int StoreID { get; set; }
        public int ID { get; set; }
        public string StoreName { get; set; }
        public string TelephoneNumber { get; set; }
        public int NumberOfProducts { get; set; }
        public DateTime? OpenDate { get; set; }
        public DateTime? DateAdded { get; set; }
        public DateTime? DateUpdate { get; set; }

    }

    public class AddStoresRequest 
    {
        public int ID { get; set; }
        public string StoreName { get; set; }
        public string TelephoneNumber { get; set; }
        public int NumberOfProducts { get; set; }
        public DateTime? OpenDate { get; set; }
    }

    public class StoreProductModel 
    {
        public int SID { get; set; }
        public int PID { get; set; }
        public bool Active { get; set; }
    }
}
