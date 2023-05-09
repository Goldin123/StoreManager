using log4net;
using ManageStoresApi.Interfaces;
using ManageStoresData.Interfaces;
using ManageStoresModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;

namespace ManageStoresApi.Implementations
{
    public class ProductApi : IProductApi
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IProductData _productData;

        public ProductApi() 
        { }
        public ProductApi(IProductData productData) 
        {
            _productData = productData;
        }

        public async Task<List<ProductDetail>> GetProductsAsync() 
        {
            try
            {
                _log.Info($"{nameof(GetProductsAsync)} attempting to get products.");
                return await _productData.GetProductsAsync();
            }
            catch (Exception ex) 
            {
                _log.Error($"{nameof(GetProductsAsync)} {ex.Message} {ex.StackTrace}.");
                return new List<ProductDetail>();
            }
            
        }
        public async Task<string> AddProductsAsync(List<ProductRequest> products) 
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