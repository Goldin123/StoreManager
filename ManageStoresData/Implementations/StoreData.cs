using Dapper;
using log4net;
using ManageStoresData.DB;
using ManageStoresData.Helpers;
using ManageStoresData.Interfaces;
using ManageStoresModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ManageStoresData.Implementations
{
    public class StoreData : IStoreData
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public async Task<List<StoreDetail>> GetStoresAsync()
        {
            try
            {
                _log.Info($"{nameof(GetStoresAsync)} attempting to get stores.");

                using (var context = new FoodLoversEntities())
                {
                    var products = context.Stores.AsQueryable();

                    var lst = (from a in products
                               select new ManageStoresModel.StoreDetail
                               {
                                   StoreID = a.SID,
                                   DateAdded = a.DateAdded,
                                   DateUpdate = a.DateUpdated,
                                   StoreName = a.StoreName,
                                   ID = (int)a.ID,
                                   NumberOfProducts = 0,
                                   OpenDate = a.OpenDate,
                                   TelephoneNumber = a.TelephoneNumber

                               }).ToList<ManageStoresModel.StoreDetail>();

                    _log.Info($"{nameof(GetStoresAsync)} successfully returned {lst.Count()} store(s).");
                    return lst;
                }

            }
            catch (Exception ex)
            {
                _log.Error($"{nameof(GetStoresAsync)} {ex.Message} {ex.StackTrace}.");
                return new List<ManageStoresModel.StoreDetail>();
            }
        }
        public async Task<StoreDetail> GetStoreByIdAsync(int Id) 
        {
            try
            {
                _log.Info($"{nameof(GetStoreByIdAsync)} attempting to get store with id {Id}.");

                using (var context = new FoodLoversEntities())
                {
                    var products = context.Stores.AsQueryable();

                    var lst = (from a in products
                               where a.ID == Id 
                               select new ManageStoresModel.StoreDetail
                               {
                                   StoreID = a.SID,
                                   DateAdded = a.DateAdded,
                                   DateUpdate = a.DateUpdated,
                                   StoreName = a.StoreName,
                                   ID = (int)a.ID,
                                   NumberOfProducts = 0,
                                   OpenDate = a.OpenDate,
                                   TelephoneNumber = a.TelephoneNumber

                               }).FirstOrDefault();

                    if ( lst != null )
                        _log.Info($"{nameof(GetStoreByIdAsync)} successfully returned {lst.StoreName} store.");
                    else
                        _log.Info($"{nameof(GetStoreByIdAsync)} no store with id {Id} found.");

                    return lst;
                }

            }
            catch (Exception ex)
            {
                _log.Error($"{nameof(GetStoreByIdAsync)} {ex.Message} {ex.StackTrace}.");
                return new ManageStoresModel.StoreDetail();
            }
        }
        public async Task<string> AddStoresAsync(List<AddStoresRequest> stores)
        {
            try
            {
                _log.Info($"{nameof(AddStoresAsync)} attempting to add store(s).");

                using (var db = new FoodLoversEntities())
                {
                    DataTable dtProduct = DataHelper.CreateStoreTypeDataTable(stores);
                    var tvp = dtProduct.AsTableValuedParameter("dbo.StoreType");
                    var parameters = new DynamicParameters();
                    parameters.Add("NewStores", tvp);
                    var response = db.Database.Connection
                           .Query("InsertStores", parameters, commandType: CommandType.StoredProcedure);

                }
                var msg = $"successfully added {stores.Count()} store(s).";
                _log.Info($"{nameof(AddStoresAsync)} {msg}");
                return msg;
            }
            catch (Exception ex)
            {
                _log.Error($"{nameof(AddStoresAsync)} {ex.Message} {ex.StackTrace}.");
                return ex.Message;
            }
        }
        public async Task<string> AddStoresProductsAsync(List<StoreProduct> storesProducts) 
        {
            try
            {
                _log.Info($"{nameof(AddStoresProductsAsync)} attempting to add store(s) and product(s).");

                using (var db = new FoodLoversEntities())
                {
                    DataTable dtProduct = DataHelper.CreateStoreProductTypeDataTable(storesProducts);
                    var tvp = dtProduct.AsTableValuedParameter("dbo.StoreProductType");
                    var parameters = new DynamicParameters();
                    parameters.Add("NewStoresProducts", tvp);
                    var response = db.Database.Connection
                           .Query("InsertStoresProducts", parameters, commandType: CommandType.StoredProcedure);

                }
                var msg = $"successfully added {storesProducts.Count()} store(s) and product(s).";
                _log.Info($"{nameof(AddStoresProductsAsync)} {msg}");
                return msg;
            }
            catch (Exception ex)
            {
                _log.Error($"{nameof(AddStoresProductsAsync)} {ex.Message} {ex.StackTrace}.");
                return ex.Message;
            }
        }
    }
}
