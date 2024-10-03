using WebKontorExpert.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebKontorExpert.BusinessLogic
{
    public interface IParentCategoryData
    {
        Task<List<ParentCategory>> GetAllParentCategories();
        Task<ParentCategory> GetParentCategoryById(int parentCategoryId);
        Task<ParentCategory> GetParentCategoryByName(string parentCategoryName);
        Task<int> AddParentCategory(ParentCategory parentCategory);
        Task UpdateParentCategory(ParentCategory parentCategory);
        Task DeleteParentCategory(int parentCategoryId);
    }
}
