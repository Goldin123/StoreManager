using ManageStoreApi.Implementations;
using ManageStoreApi.Interfaces;
using ManageStoresModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ManageStoreApi.Controllers
{
    public class StoreController : ApiController
    {
        private IStoreApi _storeApi = new StoreApi() ;
        [Route("api/Store/GetStores")]
        [HttpGet]
        public IHttpActionResult GetStores()
        {
            try
            {
                return Ok(_storeApi.GetStoresAsync());
            }
            catch
            {
                return BadRequest();
            }
        }

        [Route("api/Store/GetStoreById")]
        [HttpGet]
        public IHttpActionResult GetStoreById([FromUri] int Id)
        {
            try
            {
                return Ok(_storeApi.GetStoreByIdAsync(Id));
            }
            catch
            {
                return BadRequest();
            }
        }
        [Route("api/Store/AddStores")]
        [HttpPost]
        public IHttpActionResult AddStores([FromBody] List<AddStoresRequest> stores)
        {
            try
            {
                return Ok(_storeApi.AddStoresAsync(stores));
            }
            catch
            {
                return BadRequest();
            }
        }
        [Route("api/Store/GetStoresProducts")]
        [HttpGet]
        public IHttpActionResult GetStoresProducts()
        {
            try
            {
                return Ok(_storeApi.GetStoresProductsAsync());
            }
            catch
            {
                return BadRequest();
            }
        }


        [Route("api/Store/AddStoresProducts")]
        [HttpPost]
        public IHttpActionResult AddStores([FromBody] List<StoreProductRequest> storesProducts)
        {
            try
            {
                return Ok(_storeApi.AddStoresProductsAsync(storesProducts));
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}