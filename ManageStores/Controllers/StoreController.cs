using ManageStores.Implementations;
using ManageStores.Interfaces;
using ManageStoresModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace ManageStores.Controllers
{
    public class StoreController : Controller
    {
        readonly IProductSite _productSite;
        readonly IStoreSite _storeSite;
        readonly IStoreProductSite _storeProductSite;

        public StoreController( IStoreSite storeSite, IStoreProductSite storeProductSite, IProductSite productSite)
        {
            _storeSite = storeSite;
            _storeProductSite = storeProductSite;
            _productSite = productSite;
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

        public FileResult DownloadTemplate(int Id)
        {
            switch (Id)
            {
                case 4:
                    return File(System.IO.File.ReadAllBytes(HostingEnvironment.MapPath("~/Content/Templates/StoreCSVTemplate.csv"))
                                , System.Net.Mime.MediaTypeNames.Application.Octet
                                , Path.GetFileName(HostingEnvironment.MapPath("~/Content/Templates/StoreCSVTemplate.csv")));
                case 5:
                    return File(System.IO.File.ReadAllBytes(HostingEnvironment.MapPath("~/Content/Templates/StoreJsonTemplate.json"))
                                , System.Net.Mime.MediaTypeNames.Application.Octet
                                , Path.GetFileName(HostingEnvironment.MapPath("~/Content/Templates/StoreJsonTemplate.json")));
                case 6:
                    return File(System.IO.File.ReadAllBytes(HostingEnvironment.MapPath("~/Content/Templates/StoreXMLTemplate.xml"))
                                , System.Net.Mime.MediaTypeNames.Application.Octet
                                , Path.GetFileName(HostingEnvironment.MapPath("~/Content/Templates/StoreXMLTemplate.xml")));
                case 7:
                    return File(System.IO.File.ReadAllBytes(HostingEnvironment.MapPath("~/Content/Templates/StoreProductCSVTemplate.csv"))
                                , System.Net.Mime.MediaTypeNames.Application.Octet
                                , Path.GetFileName(HostingEnvironment.MapPath("~/Content/Templates/StoreProductCSVTemplate.csv")));
                case 8:
                    return File(System.IO.File.ReadAllBytes(HostingEnvironment.MapPath("~/Content/Templates/StoreProductJsonTemplate.json"))
                                , System.Net.Mime.MediaTypeNames.Application.Octet
                                , Path.GetFileName(HostingEnvironment.MapPath("~/Content/Templates/StoreProductJsonTemplate.json")));
                case 9:
                    return File(System.IO.File.ReadAllBytes(HostingEnvironment.MapPath("~/Content/Templates/StoreProductXMLTemplate.xml"))
                                , System.Net.Mime.MediaTypeNames.Application.Octet
                                , Path.GetFileName(HostingEnvironment.MapPath("~/Content/Templates/StoreProductXMLTemplate.xml")));
            }
            return null;
        }
    }
}