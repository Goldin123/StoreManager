using ManageStoresModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageStoresApi.Interfaces
{
    public interface IProductApi
    {
        Task<List<ProductDetail>> GetProductsAsync();
        Task<string> AddProductsAsync(List<ProductRequest> products);
        Task<string> UpdateProductsAsync(List<UpdateProductRequest> products);

    }
}
