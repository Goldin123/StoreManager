using log4net;
using ManageStoresData.DB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ManageStoresData.Helpers
{
    public static class DataHelper
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static DataTable CreateProductTypeDataTable(List<ManageStoresModel.UpdateProductRequest> products)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("PID", typeof(int));
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("ProductName", typeof(string));
            dt.Columns.Add("WeightedItem", typeof(bool));
            dt.Columns.Add("SuggestedSellingPrice", typeof(decimal));
            dt.Columns.Add("UnitsInStock", typeof(int));
            dt.Columns.Add("DateAdded", typeof(DateTime));
            dt.Columns.Add("DateUpdated", typeof(DateTime));

            foreach (var item in products)
            {
                dt.Rows.Add(
                    item.ProductID,
                    item.ID,
                    item.ProductName,
                    item.WeightedItem,
                    item.SuggestedSellingPrice,
                    0,
                    DateTime.Now,
                    DateTime.Now);
            }

            return dt;
        }
        public static DataTable CreateProductTypeDataTable(List<ManageStoresModel.AddProductRequest> products)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("PID", typeof(int));
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("ProductName", typeof(string));
            dt.Columns.Add("WeightedItem", typeof(bool));
            dt.Columns.Add("SuggestedSellingPrice", typeof(decimal));
            dt.Columns.Add("UnitsInStock", typeof(int));
            dt.Columns.Add("DateAdded", typeof(DateTime));
            dt.Columns.Add("DateUpdated", typeof(DateTime));

            foreach (var item in products)
            {
                dt.Rows.Add(0,
                    item.ID,
                    item.ProductName,
                    item.WeightedItem,
                    item.SuggestedSellingPrice,
                    0,
                    DateTime.Now,
                    DateTime.Now);
            }

            return dt;
        }
        public static DataTable CreateStoreTypeDataTable(List<ManageStoresModel.AddStoresRequest> stores)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("SID", typeof(int));
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("StoreName", typeof(string));
            dt.Columns.Add("TelephoneNumber", typeof(string));
            dt.Columns.Add("OpenDate", typeof(DateTime));
            dt.Columns.Add("DateAdded", typeof(DateTime));
            dt.Columns.Add("DateUpdated", typeof(DateTime));

            foreach (var item in stores)
            {
                dt.Rows.Add(0,
                    item.ID,
                    item.StoreName,
                    item.TelephoneNumber,
                    item.OpenDate.Date,
                    DateTime.Now,
                    DateTime.Now);
            }

            return dt;
        }
        public static DataTable CreateStoreProductTypeDataTable(List<ManageStoresModel.StoreProductRequest> storesProducts)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("SID", typeof(int));
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("Active", typeof(bool));
            dt.Columns.Add("DateAdded", typeof(DateTime));
            dt.Columns.Add("DateUpdated", typeof(DateTime));

            foreach (var item in storesProducts)
            {
                dt.Rows.Add(
                    item.SID,
                    item.PID,
                   item.Active,
                    DateTime.Now,
                    DateTime.Now);
            }

            return dt;
        }
        public static bool ValidateApiUser(string username, string password)
        {
            try 
            {
                using (var context = new FoodLoversEntities())
                {
                    return context.ApiUsers.Where(u => u.Username == username && u.Password == password).Count() > 0;
                }

            } catch (Exception ex) 
            {
                _log.Error($"{nameof(ValidateApiUser)} {ex.Message} {ex.StackTrace}.");

                return false;
            }
      
        }
    }
}
