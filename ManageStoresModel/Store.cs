using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

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

        public DateTime OpenDate { get; set; }
    }

    public class StoreProductRequest 
    {
        public int SID { get; set; }
        public int PID { get; set; }
        public bool Active { get; set; }
    }

    public class StoreDetailResponse
    {
        public List<StoreDetail> Result { get; set; }
    }

    public class StoreUpload
    {
        public int FileType { get; set; }

        [Required(ErrorMessage = "Please select file.")]
        public HttpPostedFileBase File { get; set; }
    }
    public class AddStoresResponse
    {
        public string Result { get; set; }
    }

    public class AddStoresRequestJson
    {
        public string ID { get; set; }
        [JsonProperty(PropertyName = "Name")]
        public string StoreName { get; set; }
        public string TelephoneNumber { get; set; }
        public string OpenDate { get; set; }
    }

    public class StoreProductUpload
    {
        public int FileType { get; set; }

        [Required(ErrorMessage = "Please select file.")]
        public HttpPostedFileBase File { get; set; }
    }

    public class StoreProductDetail
    {
        public int SPID { get; set; }
        public int SID { get; set; }
        public int PID { get; set; }
        public bool Active { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DateUpdated { get; set; }
    }

    public class StoreProductDetailResponse
    {
        public List<StoreProductDetail> Result { get; set; }
    }

    public class AddStoresProductsRequestJson
    {
        [JsonProperty(PropertyName = "BranchID")]
        public string SID { get; set; }
        [JsonProperty(PropertyName = "ProductID")]
        public string PID { get; set; }
    }
}
