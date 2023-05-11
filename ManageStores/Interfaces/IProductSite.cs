using ManageStoresModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageStores.Interfaces
{
    public interface IProductSite
    {
        Task<Tuple<bool, string, List<ProductDetail>>> GetProductsAsync();
        Task<Tuple<bool, string>> AddProductFileAsync(ProductUpload productUpload);
    }
}
