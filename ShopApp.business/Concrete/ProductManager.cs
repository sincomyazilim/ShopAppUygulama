using System.Collections.Generic;
using ShopApp.business.Abstract;
using ShopApp.data.Abstract;
using ShopApp.data.Concrete.EfCore;
using ShopApp.entity;

namespace ShopApp.business.Concrete
{       
     public class ProductManager : IProductService
    { // depensinh ınjection yapılıyor
        private IProductRepository _productRepository;
        public ProductManager(IProductRepository productRepository)
        {
            this._productRepository=productRepository;
        }
        // depensinh ınjection yapılıyor


        public bool Create(Product entity)
        {
            //iş kurulları uygulanablır
            if (Validation(entity))
            {
                _productRepository.Create(entity);
                return true;
            }
            return false;
            
          
        }

        public void Delete(Product entity)
        {
            _productRepository.Delete(entity);
        }

        public List<Product> GetAll()
        {
           return _productRepository.GetAll();
        }

        public Product GetById(int id)
        {
            return _productRepository.GetById(id);
        }

        public Product GetByIdWithCategories(int id)
        {
            return _productRepository.GetByIdWithCategories(id);
        }

        public int GetCountByCategory(string category)
        {
            return _productRepository.GetCountByCategory(category);
        }

        public List<Product> GetHomePageProducts()
        {
            return _productRepository.GetHomePageProducts();
        }

        public Product GetProductDetails(string url)
        {
            return _productRepository.GetProductDetails(url);
        }

        public List<Product> GetProductsByCategory(string name,int page,int pagesize)
        {
            return _productRepository.GetProductsByCategory(name, page,pagesize);
        }

        public List<Product> GetSearchResut(string searchString)
        {
            return _productRepository.GetSearchResut(searchString);
        }

        public void Update(Product entity)
        {
            _productRepository.Update(entity);
        }

        public bool Update(Product entity, int[] categoryIds)
        {   
             if (Validation(entity))
             {   
                if (categoryIds.Length==0)
                {
                    ErrorMessage+="business katmanından ProductManeger geliyorum,Ürün için en az bir kategori seçmelisiniz";
                    return false;
                }
                 _productRepository.Update(entity,categoryIds);
                 return true;
             }
             return false;

            
        }

        public string ErrorMessage { get; set; }
        public bool Validation(Product entity)
        {
            var isValid=true;
            if (string.IsNullOrEmpty(entity.Name))
            {
                ErrorMessage+="bu mesaj businesten gelıyor.ürün ismi girmelisiniz.\n";
                isValid=false;
            }
            if (entity.Price<0)
            {
                ErrorMessage+="bu mesaj businesten gelıyor.ürün Fiyatı negatif olamaz.\n";
                isValid=false;
            }
            return isValid;
            
        }
    }
}