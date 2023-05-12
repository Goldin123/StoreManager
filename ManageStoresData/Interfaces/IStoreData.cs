using ManageStoresModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageStoresData.Interfaces
{
    public interface IStoreData
    {
        Task<List<StoreDetail>> GetStoresAsync();
        Task<StoreDetail> GetStoreByIdAsync(int Id);
        Task<string> AddStoresAsync(List<AddStoresRequest> stores);
        Task<string> AddStoresProductsAsync(List<StoreProductRequest> storesProducts);
        Task<List<StoreProductDetail>> GetStoreProductsAsync();
    }
}
