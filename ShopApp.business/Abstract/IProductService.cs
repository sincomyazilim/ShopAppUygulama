using System.Collections.Generic;
using ShopApp.entity;

namespace ShopApp.business.Abstract
{
    public interface IProductService:IValidator<Product>
    { 
        Product GetById(int id);
        Product GetByIdWithCategories(int id);
        Product GetProductDetails(string url);
        List<Product> GetProductsByCategory(string name,int page,int pagesize);
       
        List<Product> GetAll();
        
        bool Create (Product entity);

        void Update (Product entity);

        void Delete (Product entity);
        int GetCountByCategory(string category);
        List<Product> GetHomePageProducts();
        List<Product> GetSearchResut(string searchString);
        bool Update(Product entity, int[] categoryIds);
    }
}