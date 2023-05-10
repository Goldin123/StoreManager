using ManageStoresModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageStoresData.Interfaces
{
    public interface IProductData
    {
        Task<List<ProductDetail>> GetProductsAsync();
        Task<ProductDetail> GetProductByIdAsync(int Id);
        Task<string> AddProductsAsync(List<AddProductRequest> products);
        Task<string> UpdateProductsAsync(List<UpdateProductRequest> products);
    }
}
