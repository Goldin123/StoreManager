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
    public class ProductDetail
    {
        public int ProductID { get; set; }
        public int ID { get; set; }
        public string ProductName { get; set; }
        public bool? WeightedItem { get; set; }
        public decimal? SuggestedSellingPrice { get; set; }
        public DateTime? DateAdded { get; set; }
        public DateTime? DateUpdated { get; set; }

        public ProductDetail() 
        {

        }

        public ProductDetail(int productID, string productName, bool? weightedItem, decimal? suggestedSellingPrice, DateTime dateAdded, DateTime dateUpdated)
        {
            ProductID = productID;
            ProductName = productName;
            WeightedItem = weightedItem;
            SuggestedSellingPrice = suggestedSellingPrice;
            DateAdded = dateAdded;
            DateUpdated = dateUpdated;
        }
    }

    public class ProductDetailResponse 
    {
        public List<ProductDetail> Result  { get; set; }
    }

    public class AddProductRequest 
    {
        public int ID { get; set; }
        public string ProductName { get; set; }
        public bool? WeightedItem { get; set; }
        public decimal? SuggestedSellingPrice { get; set; }
    }

    public class AddProductRequestJson
    {
        public string ID { get; set; }
        [JsonProperty(PropertyName = "Name")]
        public string ProductName { get; set; }
        public string WeightedItem { get; set; }
        public string SuggestedSellingPrice { get; set; }
    }

    public class UpdateProductRequest
    {
        public int ProductID { get; set; }
        public int ID { get; set; }
        public string ProductName { get; set; }
        public bool? WeightedItem { get; set; }
        public decimal? SuggestedSellingPrice { get; set; }
    }

    public class AddProductResponse
    {
        public string Result { get; set; }
    }

    public class ProductUpload 
    {
        public int FileType { get; set; }

        [Required(ErrorMessage = "Please select file.")]
        public HttpPostedFileBase File { get; set; }
    }

}
