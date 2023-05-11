﻿using ManageStores.Interfaces;
using ManageStoresModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
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

        public async Task<ActionResult> AddProduct()
        {
           
            return View(new ProductUpload());
        }

        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddProductToInventory(ProductUpload product)
        {
            if (ModelState.IsValid)
            {
                var result = await _productSite.AddProductFileAsync(product);
                //if (result.Item1)
                //{
                //    var products = await _productRepository.GetProducts();
                //    if (products.Item1 != null)
                //        Globals.TotalProducts = products.Item1.Count();

                //    TempData["ProductSuccess"] = "product added successfully.";
                //}
            }

            return RedirectToAction("AddProduct");
        }
    }
}