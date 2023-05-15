
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ShopApp.business.Abstract;
using ShopApp.data.Abstract;
using ShopApp.webui.Models;

namespace ShopApp.webuii.Controllers
{
    public class HomeController:Controller
    {   private IProductService _productService;//depesing ınjection tanımlıyoruz

        public HomeController(IProductService _productService)//burda ne zaman clasımız newlenırse ona yukardakı ver dıyoruz startuo sayfasınada ekleme yapacaz
        {
           this._productService=_productService;
        }
        public IActionResult Index()
        {
           
             var productViewModel=new ProductListViewModel()
             {     
                   
                   Products=_productService.GetHomePageProducts()
                  
             };

            
            return View(productViewModel);
        }
        public IActionResult About()
        {
            return View();
        }
        public IActionResult Contac()
        {
            return View("MyView");
        }
    }
}