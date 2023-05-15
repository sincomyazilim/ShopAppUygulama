using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ShopApp.data.Abstract;
using ShopApp.entity;

namespace ShopApp.data.Concrete.EfCore
{
    public class EfCoreCategoryRepository : EfCoreGenericRepository<Category, ShopContext>, ICategoryRepository //ICategoryRepository bunu ekleme sebebımız ıse genel metodlar dısında exra medod ıcerıyorsa onuda uygulayabılmek ıcın
    {
        public void DeleteFromCategory(int productId, int categoryId)
        {
            using (var context=new ShopContext())
            {
                var cmd="Delete from productcategory where ProductId=@p0 and CategoryId=@p1";
                context.Database.ExecuteSqlRaw(cmd,productId,categoryId);
            }
        }

        public Category GetByIdWithProducts(int categoryId)
        {
          using (var context=new ShopContext())
          {
              return context.Categories
                            .Where(i=>i.CategoryId==categoryId)
                            .Include(i=>i.ProductCategories)
                            .ThenInclude(i=>i.Product)
                            .FirstOrDefault();
          }
        }

        // public List<Category> GetPopularCategories()// genel dısında tanmlanan metod burda tanımlanıyor
        // {
        //     throw new System.NotImplementedException();
        // }
    }
}