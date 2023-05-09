using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageStoresModel
{
    public class ProductDetail
    {
        public int ProductID { get; set; }
        public int ID { get; set; }
        public string ProductName { get; set; }
        public bool? WeightedItem { get; set; }
        public decimal? SuggestedSellingPrice { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DateUpdated { get; set; }

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

    public class ProductRequest 
    {
        public int ID { get; set; }
        public string ProductName { get; set; }
        public bool? WeightedItem { get; set; }
        public decimal? SuggestedSellingPrice { get; set; }
    }

    public class UpdateProductRequest
    {
        public int ProductID { get; set; }
        public int ID { get; set; }
        public string ProductName { get; set; }
        public bool? WeightedItem { get; set; }
        public decimal? SuggestedSellingPrice { get; set; }
    }
}
