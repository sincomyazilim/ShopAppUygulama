using System.Collections.Generic;
using ShopApp.business.Abstract;
using ShopApp.data.Abstract;
using ShopApp.entity;

namespace ShopApp.business.Concrete
{
    public class CategoryManager : ICategoryService
    { // depensinh ýnjection yapýlýyor
        private ICategoryRepository _categoryRepository;
        public CategoryManager(ICategoryRepository categoryRepository)
        {
            this._categoryRepository=categoryRepository;
        }
        // depensinh ýnjection yapýlýyor

        public void Create(Category entity)
        {
            _categoryRepository.Create(entity);
        }

        public void Delete(Category entity)
        {
            _categoryRepository.Delete(entity);
        }

        public void DeleteFromCategory(int productId, int categoryId)
        {
            _categoryRepository.DeleteFromCategory(productId,categoryId);
        }

        public List<Category> GetAll()
        {
            return _categoryRepository.GetAll();
        }

        public Category GetById(int id)
        {
            return _categoryRepository.GetById(id);
        }

        public Category GetByIdWithProducts(int categoryId)
        {
            return _categoryRepository.GetByIdWithProducts(categoryId);
        }

        public void Update(Category entity)
        {
            _categoryRepository.Update(entity);
        }
        public string ErrorMessage { get; set; }

        public bool Validation(Category entity)
        {
            throw new System.NotImplementedException();
        }
    }
}