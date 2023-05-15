using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ShopApp.entity;

namespace ShopApp.webui.Models
{
    public class ProductModel
    {
        public int  ProductId { get; set; }
        // [Display(Name="Name",Prompt="Enter product name")]
        // [Required(ErrorMessage="Name zorunlu bir alan")]
        // [StringLength(60,MinimumLength=5,ErrorMessage="Ürün ismi 5-10 karekter aralığnda olmalı")]
        public string Name { get; set; }
        

        [Required(ErrorMessage="Url zorunlu bir alan")]
         [Display(Name="Url",Prompt="Enter product Url")]
        public string Url { get; set; }
        
        // [Range(1,10000,ErrorMessage="Fiyat aralağı 1 ile 10000 arasında olabılır")]
        //  [Required(ErrorMessage="Price zorunlu bir alan")]
        //   [Display(Name="Price",Prompt="Enter product Price")]
        public double? Price { get; set; }
       

        [Required(ErrorMessage="ImageUrl zorunlu bir alan")]
         [Display(Name="ImageUrl",Prompt="Enter product ImageUrl")]
        public string ImageUrl { get; set; }
        

        [Required(ErrorMessage="Description zorunlu bir alan")]
         [Display(Name="Description",Prompt="Enter product Description")]
        public string Description { get; set; }
        
        public bool IsApproved { get; set; }
        public bool IsHome { get; set; }
        public List<Category> SelectedCategories{ get; set; }
      
    }
}