using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ShopApp.business.Abstract;

namespace ShopApp.webui.ViewComponents
{
    public class CategoriesViewComponent:ViewComponent
    {

        private ICategoryService _categoryService;//depesing ınjection tanımlıyoruz

        public CategoriesViewComponent(ICategoryService categoryService)//burda ne zaman clasımız newlenırse ona yukardakı ver dıyoruz startuo sayfasınada ekleme yapacaz
        {
           this._categoryService=categoryService;
        }

        public IViewComponentResult Invoke()
        {
             if (RouteData.Values["category"]!=null)
             {
                 ViewBag.SecilenId=RouteData?.Values["category"];
             }
             if (RouteData.Values["action"]!=null && RouteData.Values["category"]==null)
             {
                     ViewBag.SecilenIdYok=1;
             }
          
             return View(_categoryService.GetAll());
           
        }
    }
}