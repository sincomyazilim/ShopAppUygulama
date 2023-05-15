using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ShopApp.business.Abstract;
using ShopApp.entity;
using ShopApp.webui.Models;


namespace ShopApp.webui.Controllers
{
    public class ShopController:Controller
    {
         private IProductService _productService;//depesing ınjection tanımlıyoruz

        public ShopController(IProductService _productService)//burda ne zaman clasımız newlenırse ona yukardakı ver dıyoruz startuo sayfasınada ekleme yapacaz
        {
           this._productService=_productService;
        }


          public IActionResult List(string category,int page=1)
        {
           const int pageSize=2;
             var productViewModel=new ProductListViewModel()
             {     
                   PageInfo=new PageInfo()
                   {
                       TotalItems=_productService.GetCountByCategory(category),
                       CurrentPage=page,
                       ItemsPerPage=pageSize,
                       CurrentCategory=category
                   },
                   Products=_productService.GetProductsByCategory(category,page,pageSize)
                  
             };

            
            return View(productViewModel);
        }

        public IActionResult Details(string url)
        {
            if (url==null)
            {
                return NotFound();
            }
            
            Product product=_productService.GetProductDetails(url);
            if (product==null)
            {
                return NotFound();
            }
            return View(new ProductDetailModel{
                Product=product,
                Categories=product.ProductCategories.Select(i=>i.Category).ToList()
            });

        }

        public IActionResult Search(string q)
        {
            
             var productViewModel=new ProductListViewModel()
             {     
                   
                   Products=_productService.GetSearchResut(q)
                  
             };

            
            return View(productViewModel);

        }



    }
}