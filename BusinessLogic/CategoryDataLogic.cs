using WebKontorExpert.DataAccess;
using WebKontorExpert.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebKontorExpert.BusinessLogic
{
    public class CategoryDataLogic : ICategoryData
    {
        private readonly ICategoryAccess _categoryAccess;

        public CategoryDataLogic(ICategoryAccess categoryAccess)
        {
            _categoryAccess = categoryAccess;
        }

        public async Task<List<Category>> GetAllCategories()
        {
            return await _categoryAccess.GetAllCategories();
        }

        public async Task<Category> GetCategoryById(int categoryId)
        {
            return await _categoryAccess.GetCategoryById(categoryId);
        }

        public async Task<Category> GetCategoryByName(string categoryName)
        {
            return await _categoryAccess.GetCategoryByName(categoryName);
        }

        public async Task<List<Category>> GetCategoriesByParentCategoryId(int parentCategoryId)
        {
            return await _categoryAccess.GetCategoriesByParentCategoryId(parentCategoryId);
        }

        public async Task<int> AddCategory(Category category)
        {
            return await _categoryAccess.AddCategory(category);
        }

        public async Task UpdateCategory(Category category)
        {
            await _categoryAccess.UpdateCategory(category);
        }

        public async Task DeleteCategory(int categoryId)
        {
            await _categoryAccess.DeleteCategory(categoryId);
        }
    }
}
