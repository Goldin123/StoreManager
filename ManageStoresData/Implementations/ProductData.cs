using Dapper;
using log4net;
using ManageStoresData.DB;
using ManageStoresData.Helpers;
using ManageStoresData.Interfaces;
using ManageStoresModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Contexts;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace ManageStoresData.Implementations
{
    public class ProductData : IProductData
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public async Task<List<ManageStoresModel.ProductDetail>> GetProductsAsync()
        {
            try
            {
                _log.Info($"{nameof(GetProductsAsync)} attempting to get products.");

                using (var context = new FoodLoversEntities())
                {
                    var products = context.Products.AsQueryable();

                    var lst = (from a in products
                               select new ManageStoresModel.ProductDetail
                               {
                                   ID = (int)a.ID,
                                   ProductID = a.PID,
                                   ProductName = a.ProductName,
                                   WeightedItem = a.WeightedItem,
                                   SuggestedSellingPrice = a.SuggestedSellingPrice,
                                   DateAdded = a.DateAdded,
                                   DateUpdated = a.DateUpdated

                               }).ToList<ManageStoresModel.ProductDetail>();

                    _log.Info($"{nameof(GetProductsAsync)} successfully returned {lst.Count()} product(s).");
                    return lst;
                }

            }
            catch (Exception ex)
            {
                _log.Error($"{nameof(GetProductsAsync)} {ex.Message} {ex.StackTrace}.");
                return new List<ManageStoresModel.ProductDetail>();
            }
        }
        public async Task<string> AddProductsAsync(List<ManageStoresModel.AddProductRequest> products)
        {
            try
            {
                _log.Info($"{nameof(AddProductsAsync)} attempting to add products.");

                using (var db = new FoodLoversEntities())
                {
                    DataTable dtProduct = DataHelper.CreateProductTypeDataTable(products);
                    var tvp = dtProduct.AsTableValuedParameter("dbo.ProductType");
                    var parameters = new DynamicParameters();
                    parameters.Add("NewProducts", tvp);
                    var response = db.Database.Connection
                           .Query("InsertProducts", parameters, commandType: CommandType.StoredProcedure);

                }
                var msg = $"successfully added {products.Count()} product(s).";
                _log.Info($"{nameof(UpdateProductsAsync)} {msg}");
                return msg;
            }
            catch (Exception ex)
            {
                _log.Error($"{nameof(AddProductsAsync)} {ex.Message} {ex.StackTrace}.");
                return ex.Message;
            }
        }
        public async Task<string> UpdateProductsAsync(List<ManageStoresModel.UpdateProductRequest> products)
        {
            try
            {
                _log.Info($"{nameof(UpdateProductsAsync)} attempting to update products.");

                using (var db = new FoodLoversEntities())
                {
                    DataTable dtProduct = DataHelper.CreateProductTypeDataTable(products);
                    var tvp = dtProduct.AsTableValuedParameter("dbo.ProductType");
                    var parameters = new DynamicParameters();
                    parameters.Add("ProductsToUpdate", tvp);
                    var response = db.Database.Connection
                           .Query("UpdateProducts", parameters, commandType: CommandType.StoredProcedure);

                }
                var msg = $"successfully updated {products.Count()} product(s).";
                _log.Info($"{nameof(UpdateProductsAsync)} {msg}");
                return msg;
            }
            catch (Exception ex)
            {
                _log.Error($"{nameof(UpdateProductsAsync)} {ex.Message} {ex.StackTrace}.");
                return ex.Message;
            }
        }
        public async Task<ProductDetail> GetProductByIdAsync(int id)
        {
            try
            {
                _log.Info($"{nameof(GetProductByIdAsync)} attempting to get product with id {id}.");

                using (var context = new FoodLoversEntities())
                {
                    var products = context.Products.AsQueryable();

                    var prod = (from a in products
                               where a.ID == id
                               select new ManageStoresModel.ProductDetail
                               {
                                   ID = (int)a.ID,
                                   ProductID = a.PID,
                                   ProductName = a.ProductName,
                                   WeightedItem = a.WeightedItem,
                                   SuggestedSellingPrice = a.SuggestedSellingPrice,
                                   DateAdded = a.DateAdded,
                                   DateUpdated = a.DateUpdated
                               }).FirstOrDefault();
                    if (prod != null)
                        _log.Info($"{nameof(GetProductByIdAsync)} successfully returned {prod.ProductName} product.");
                    else
                        _log.Info($"{nameof(GetProductByIdAsync)} no product with id {id} found.");
                    return prod;
                }

            }
            catch (Exception ex)
            {
                _log.Error($"{nameof(GetProductByIdAsync)} {ex.Message} {ex.StackTrace}.");
                return new ManageStoresModel.ProductDetail();
            }

        }

    }
}
