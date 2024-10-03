using WebKontorExpert.Models;

namespace WebKontorExpert.DataAccess
{
    public interface ICategoryAccess
    {
        Task<List<Category>> GetAllCategories();
        Task<Category> GetCategoryById(int categoryId);
        Task<Category> GetCategoryByName(string categoryName);
        Task<List<Category>> GetCategoriesByParentCategoryId(int parentCategoryId);
        Task<int> AddCategory(Category category);
        Task UpdateCategory(Category category);
        Task DeleteCategory(int categoryId);
    }
}
