using System.Linq;
using Microsoft.EntityFrameworkCore;
using ShopApp.entity;

namespace ShopApp.data.Concrete.EfCore
{
    public class SeedDatabase
    {
        public static void Seed()
        {

            var context=new ShopContext();
            if (context.Database.GetPendingMigrations().Count()==0)
            {
                if (context.Categories.Count()==0)
                {
                    context.Categories.AddRange(Categories);
                }
                  if (context.Products.Count()==0)
                {
                    context.Products.AddRange(Products);
                    context.AddRange(ProductCategories);//cansın tablosunu doldurduk
                }
            }
            context.SaveChanges();
            
        }
        private static Category[] Categories={
            new Category(){Name="Telefon",Url="telefon"},
            new Category(){Name="Bilgisayar",Url="bilgisayar"},
            new Category(){Name="Elektronik",Url="elektronik"},
            new Category(){Name="Beyaz Eşya",Url="beyaz-esya"},
        }; 
        private static Product[] Products={
            new Product(){Name="samsun s1",Url="samsung-s1",Price=1000,Description="Güzel Telefon",IsApproved=true,ImageUrl="1.jpg"},
            new Product(){Name="samsun s2",Url="samsung-s2",Price=2000,Description="Güzel Telefon",IsApproved=false,ImageUrl="2.jpg"},
            new Product(){Name="samsun s3",Url="samsung-s3",Price=3000,Description="Güzel Telefon",IsApproved=true,ImageUrl="3.jpg"},
            new Product(){Name="samsun s4",Url="samsung-s4",Price=4000,Description="Güzel Telefon",IsApproved=false,ImageUrl="4.jpg"},
            new Product(){Name="samsun s5",Url="samsung-s5",Price=5000,Description="Güzel Telefon",IsApproved=true,ImageUrl="2.jpg"},
            new Product(){Name="samsun s6",Url="samsung-s6",Price=6000,Description="Güzel Telefon",IsApproved=true,ImageUrl="4.jpg"},
            new Product(){Name="samsun s7",Url="samsung-s7",Price=7000,Description="Güzel Telefon",IsApproved=false,ImageUrl="1.jpg"},
            new Product(){Name="samsun s8",Url="samsung-s8",Price=8000,Description="Güzel Telefon",IsApproved=true,ImageUrl="3.jpg"},
            new Product(){Name="samsun s9",Url="samsung-s9",Price=9000,Description="Güzel Telefon",IsApproved=true,ImageUrl="9.jpg"},
          
        };

        private static ProductCategory[] ProductCategories={
            
            new ProductCategory(){Product=Products[0],Category=Categories[0]},
            new ProductCategory(){Product=Products[0],Category=Categories[2]},
            new ProductCategory(){Product=Products[1],Category=Categories[0]},
            new ProductCategory(){Product=Products[1],Category=Categories[2]},
            new ProductCategory(){Product=Products[2],Category=Categories[0]},
            new ProductCategory(){Product=Products[2],Category=Categories[2]},
            new ProductCategory(){Product=Products[3],Category=Categories[0]},
            new ProductCategory(){Product=Products[3],Category=Categories[2]}
          

        };
        
        
        
    }
}