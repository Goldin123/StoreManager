using log4net;
using ManageStoreApi.Interfaces;
using ManageStoresData.Implementations;
using ManageStoresData.Interfaces;
using ManageStoresModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;

namespace ManageStoreApi.Implementations
{
    public class StoreApi: IStoreApi
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IStoreData _storeData = new StoreData();
        public async Task<List<StoreDetail>> GetStoresAsync()
        {
            try
            {
                return await _storeData.GetStoresAsync();
            }
            catch (Exception ex)
            {
                _log.Error($"{nameof(GetStoresAsync)} {ex.Message} {ex.StackTrace}.");
                return new List<StoreDetail>();
            }
        }
        public async Task<StoreDetail> GetStoreByIdAsync(int Id)
        {
            try
            {
                return await _storeData.GetStoreByIdAsync(Id);
            }
            catch (Exception ex)
            {
                _log.Error($"{nameof(GetStoreByIdAsync)} {ex.Message} {ex.StackTrace}.");
                return new StoreDetail();
            }
        }
        public async Task<string> AddStoresAsync(List<AddStoresRequest> stores)
        {
            try
            {
                return await _storeData.AddStoresAsync(stores);
            }
            catch (Exception ex)
            {
                _log.Error($"{nameof(AddStoresAsync)} {ex.Message} {ex.StackTrace}.");
                return ex.Message;
            }
        }

        public async Task<string> AddStoresProductsAsync(List<StoreProductRequest> storesProducts)
        {
            try
            {
                return await _storeData.AddStoresProductsAsync(storesProducts);
            }
            catch (Exception ex)
            {
                _log.Error($"{nameof(AddStoresProductsAsync)} {ex.Message} {ex.StackTrace}.");
                return ex.Message;
            }
        }
        public async Task<List<StoreProductDetail>> GetStoresProductsAsync()
        {
            try
            {
                return await _storeData.GetStoreProductsAsync();
            }
            catch (Exception ex)
            {
                _log.Error($"{nameof(GetStoresAsync)} {ex.Message} {ex.StackTrace}.");
                return new List<StoreProductDetail>();
            }
        }
    }
}