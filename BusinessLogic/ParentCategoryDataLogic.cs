using WebKontorExpert.DataAccess;
using WebKontorExpert.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebKontorExpert.BusinessLogic
{
    public class ParentCategoryDataLogic : IParentCategoryData
    {
        private readonly IParentCategoryAccess _parentCategoryAccess;

        public ParentCategoryDataLogic(IParentCategoryAccess parentCategoryAccess)
        {
            _parentCategoryAccess = parentCategoryAccess;
        }

        public async Task<List<ParentCategory>> GetAllParentCategories()
        {
            return await _parentCategoryAccess.GetAllParentCategories();
        }

        public async Task<ParentCategory> GetParentCategoryById(int parentCategoryId)
        {
            return await _parentCategoryAccess.GetParentCategoryById(parentCategoryId);
        }

        public async Task<ParentCategory> GetParentCategoryByName(string parentCategoryName)
        {
            return await _parentCategoryAccess.GetParentCategoryByName(parentCategoryName);
        }

        public async Task<int> AddParentCategory(ParentCategory parentCategory)
        {
            return await _parentCategoryAccess.AddParentCategory(parentCategory);
        }

        public async Task UpdateParentCategory(ParentCategory parentCategory)
        {
            await _parentCategoryAccess.UpdateParentCategory(parentCategory);
        }

        public async Task DeleteParentCategory(int parentCategoryId)
        {
            await _parentCategoryAccess.DeleteParentCategory(parentCategoryId);
        }
    }
}
