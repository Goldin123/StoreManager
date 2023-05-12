using ManageStoresModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageStores.Interfaces
{
    public interface IStoreProductSite
    {
        Task<Tuple<bool, string, List<StoreProductDetail>>> GetStoresProductsAsync();
        Task<Tuple<bool, string>> AddStoreProductFileAsync(StoreUpload storeUpload);
    }
}
