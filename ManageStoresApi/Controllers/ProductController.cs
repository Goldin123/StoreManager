using ManageStoresApi.Implementations;
using ManageStoresApi.Interfaces;
using ManageStoresData.Implementations;
using ManageStoresData.Interfaces;
using ManageStoresModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ManageStoresApi.Controllers
{
    public class ProductController : ApiController
    {
       private IProductApi _productApi;

        public ProductController()
        {
            IProductData productData = new ProductData();
            _productApi = new ProductApi(productData);
        }
      
        [Route("api/Product/GetProducts")]
        [HttpGet]
        public IHttpActionResult GetProducts()
        {
            try
            {
                return Ok(_productApi.GetProductsAsync());
            }
            catch
            {
                return BadRequest();
            }
        }
        [Route("api/Product/GetProductById")]
        [HttpGet]
        public IHttpActionResult GetProductById([FromUri]int Id)
        {
            try
            {
                return Ok(_productApi.GetProductsByIdAsync(Id));
            }
            catch
            {
                return BadRequest();
            }
        }

        [Route("api/Product/AddProducts")]
        [HttpPost]
        public IHttpActionResult AddProducts([FromBody] List<AddProductRequest> products)
        {
            try
            {
                return Ok(_productApi.AddProductsAsync(products));
            }
            catch
            {
                return BadRequest();
            }
        }
        [Route("api/Product/UpdateProducts")]
        [HttpPut]
        public IHttpActionResult UpdateProducts([FromBody] List<UpdateProductRequest> products)
        {
            try
            {
                return Ok(_productApi.UpdateProductsAsync(products));
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}