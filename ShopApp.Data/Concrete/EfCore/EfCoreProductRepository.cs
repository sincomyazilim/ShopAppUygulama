using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ShopApp.data.Abstract;
using ShopApp.entity;

namespace ShopApp.data.Concrete.EfCore
{
    public class EfCoreProductRepository : EfCoreGenericRepository<Product, ShopContext>, IProductRepository//IProductRepositor bunu ekleme sebebımız ıse genel metodlar dısında exra medod ıcerıyorsa onuda uygulayabılmek ıcın
    {
        public Product GetByIdWithCategories(int id)
        {
            using (var context=new ShopContext())
            {
                return context.Products
                               .Where(p=>p.ProductId==id)
                               .Include(i=>i.ProductCategories)
                               .ThenInclude(i=>i.Category)
                               .FirstOrDefault();
            }
        }

        public int GetCountByCategory(string category)
        {
             using (var context=new ShopContext())
            {
                var urunler =context.Products.Where(i=>i.IsApproved).AsQueryable();  //sorgu calısmadıgı ıcın bekletıyor
                if (!string.IsNullOrEmpty(category))
                {
                    urunler=urunler.Include(i=>i.ProductCategories)
                                   .ThenInclude(i=>i.Category)
                                   .Where(i=>i.ProductCategories.Any(a=>a.Category.Url==category));
                            
                }
                return urunler.Count();
            }
        }

        public List<Product> GetHomePageProducts()
        {
            using(var context=new ShopContext())
            {
                return context.Products
                              .Where(i=>i.IsApproved==true&&i.IsHome==true)
                              .ToList();
            }

        }

       

        public Product GetProductDetails(string url)
        {
             using (var context=new ShopContext())
            {
                return context.Products
                            .Where(p=>p.Url==url)
                            .Include(i=>i.ProductCategories)
                            .ThenInclude(i=>i.Category)
                            .FirstOrDefault();
            }
        }

       

        public List<Product> GetProductsByCategory(string name,int page,int pagesize)
        {
            using (var context=new ShopContext())
            {
                var urunler =context.Products
                                    .Where(i=>i.IsApproved)
                                    .AsQueryable();  //sorgu calısmadıgı ıcın bekletıyor
                if (!string.IsNullOrEmpty(name))
                {
                    urunler=urunler.Include(i=>i.ProductCategories)
                                   .ThenInclude(i=>i.Category)
                                   .Where(i=>i.ProductCategories.Any(a=>a.Category.Url==name));
                            
                }
                return urunler.Skip((page-1)*pagesize).Take(pagesize).ToList();
            }
        }

        public List<Product> GetSearchResut(string searchString)
        { 
            using (var context=new ShopContext())
            {
                var urunler =context.Products
                                    .Where(i=>i.IsApproved&&(i.Name.ToLower().Contains(searchString.ToLower())||i.Description.ToLower().Contains(searchString.ToLower())))
                                    .AsQueryable();  //sorgu calısmadıgı ıcın bekletıyor
              
                return urunler.ToList();
            }
            
        }

        public void Update(Product entity, int[] categoryIds)
        {
            /// <summary>
            /// bu update  te aynı anda produst ıle productcategory tablolarına verı eklemek
            /// ıcın kulanılıyor
            /// products.ProductCategories=categoryIds.Select(catid=>new ProductCategory()
            /// 3.satrda yaılan sey sudur :productId bellı olan urunlere aıt kategorler dızı halınde eklenıyor
            /// </summary>
            /// <returns></returns>
           using (var context=new ShopContext())
           {
               var products=context.Products
                                    .Include(i=>i.ProductCategories)
                                    .FirstOrDefault(i=>i.ProductId==entity.ProductId);
            
            if (products!=null)
            {
                products.Name=entity.Name;
                products.Description=entity.Description;
                products.Price=entity.Price;
                products.Url=entity.Url;
                products.ImageUrl=entity.ImageUrl;
                products.IsApproved=entity.IsApproved;
                products.IsHome=entity.IsHome;

                products.ProductCategories=categoryIds.Select(catid=>new ProductCategory()
                {
                    ProductId=entity.ProductId,
                    CategoryId=catid

                }).ToList();
                context.SaveChanges();
            }
           }
        }
    }
}