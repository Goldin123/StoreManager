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
using WebGrease;

namespace ManageStoreApi.Implementations
{
    public class ProductApi : IProductApi
    {
        private static readonly ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IProductData _productData = new ProductData();
        public async Task<List<ProductDetail>> GetProductsAsync()
        {
            try
            {
                return await _productData.GetProductsAsync();
            }
            catch (Exception ex)
            {
                _log.Error($"{nameof(GetProductsAsync)} {ex.Message} {ex.StackTrace}.");
                return new List<ProductDetail>();
            }
        }
        public async Task<ProductDetail> GetProductsByIdAsync(int Id)
        {
            try
            {
                return await _productData.GetProductByIdAsync(Id);
            }
            catch (Exception ex)
            {
                _log.Error($"{nameof(GetProductsByIdAsync)} {ex.Message} {ex.StackTrace}.");
                return new ProductDetail();
            }
        }
        public async Task<string> AddProductsAsync(List<AddProductRequest> products)
        {
            try
            {
                return await _productData.AddProductsAsync(products);
            }
            catch (Exception ex)
            {
                _log.Error($"{nameof(AddProductsAsync)} {ex.Message} {ex.StackTrace}.");
                return ex.Message;
            }

        }
        public async Task<string> UpdateProductsAsync(List<UpdateProductRequest> products)
        {
            try
            {
                return await _productData.UpdateProductsAsync(products);
            }
            catch (Exception ex)
            {
                _log.Error($"{nameof(UpdateProductsAsync)} {ex.Message} {ex.StackTrace}.");
                return ex.Message;
            }

        }
    }
}