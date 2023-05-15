using System.Collections.Generic;
using ShopApp.entity;

namespace ShopApp.data.Abstract
{
    public interface ICategoryRepository:IRepository<Category>
    {
         //List<Category> GetPopularCategories();
         Category GetByIdWithProducts(int categoryId);
         void DeleteFromCategory(int productId,int categoryId);
    }
}