using ManageStoresModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageStores.Interfaces
{
    public interface IStoreSite
    {
        Task<Tuple<bool, string, List<StoreDetail>>> GetStoresAsync();
        Task<Tuple<bool, string>> AddStoreFileAsync(StoreUpload storeUpload);
    }
}
