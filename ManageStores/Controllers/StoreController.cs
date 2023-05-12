using ManageStores.Implementations;
using ManageStores.Interfaces;
using ManageStoresModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ManageStores.Controllers
{
    public class StoreController : Controller
    {
        readonly IStoreSite _storeSite;
        readonly IStoreProductSite _storeProductSite;

        public StoreController(IStoreSite storeSite, IStoreProductSite storeProductSite)
        {
            _storeSite = storeSite;
            _storeProductSite = storeProductSite;
        }


        // GET: Store
        public async Task<ActionResult> Index()
        {
            var stores = await _storeSite.GetStoresAsync();
            if (stores != null)
                if (stores.Item1)
                    return View(stores.Item3);
            return View(new List<StoreDetail>());
        }

        public async Task<ActionResult> AddStores()=>View(new StoreUpload());

        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddStoresToInventory(StoreUpload stores)
        {
            if (ModelState.IsValid)
            {
                var result = await _storeSite.AddStoreFileAsync(stores);
                if (result.Item1)
                {
                    TempData["UploadStatus"] = result.Item2;
                }
                else
                {
                    TempData["UploadError"] = result.Item2;
                }
            }
            else
            {
                if (stores.File == null)
                    TempData["UploadError"] = "Please select a file.";
            }

            return RedirectToAction("AddStores");
        }

        public async Task<ActionResult> AddStoresProductsToInventory(StoreUpload stores)
        {
            if (ModelState.IsValid)
            {
                var result = await _storeProductSite.AddStoreProductFileAsync(stores);
                if (result.Item1)
                {
                    TempData["UploadStatus"] = result.Item2;
                }
                else
                {
                    TempData["UploadError"] = result.Item2;
                }
            }
            else
            {
                if (stores.File == null)
                    TempData["UploadError"] = "Please select a file.";
            }

            return RedirectToAction("AddStores");
        }


        public async Task<ActionResult> AddStoresProducts() => View(new StoreProductUpload());
    }
}