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
    public class ProductController : Controller
    {
        readonly IProductSite _productSite;
        // GET: Product
        
        public ProductController(IProductSite productSite) 
        {
            _productSite = productSite;
        }

        public async Task<ActionResult> Index()
        {
            var products = await _productSite.GetProductsAsync();
            if (products != null)
                if (products.Item1)
                    return View(products.Item3);
            return View(new List<ProductDetail>());
        }

        public async Task<ActionResult> AddProduct() => View(new ProductUpload());

        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddProductToInventory(ProductUpload product)
        {
            if (ModelState.IsValid)
            {
                var result = await _productSite.AddProductFileAsync(product);
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
                if (product.File ==null)
                    TempData["UploadError"] = "Please select a file.";
            }

            return RedirectToAction("AddProduct");
        }

        public FileResult DownloadTemplate(int Id)
        {
            switch (Id)
            {
                case 1:
                    return File(System.IO.File.ReadAllBytes(HostingEnvironment.MapPath("~/Content/Templates/ProductCSVTemplate.csv"))
                                , System.Net.Mime.MediaTypeNames.Application.Octet
                                , Path.GetFileName(HostingEnvironment.MapPath("~/Content/Templates/ProductCSVTemplate.csv")));
                case 2:
                    return File(System.IO.File.ReadAllBytes(HostingEnvironment.MapPath("~/Content/Templates/ProductJsonTemplate.json"))
                                , System.Net.Mime.MediaTypeNames.Application.Octet
                                , Path.GetFileName(HostingEnvironment.MapPath("~/Content/Templates/ProductJsonTemplate.json")));
                case 3:
                    return File(System.IO.File.ReadAllBytes(HostingEnvironment.MapPath("~/Content/Templates/ProductXMLTemplate.xml"))
                                , System.Net.Mime.MediaTypeNames.Application.Octet
                                , Path.GetFileName(HostingEnvironment.MapPath("~/Content/Templates/ProductXMLTemplate.xml")));
            }
            return null;
        }
    }
}