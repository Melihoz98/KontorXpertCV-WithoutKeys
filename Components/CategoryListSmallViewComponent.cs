using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebKontorExpert.BusinessLogic;
using WebKontorExpert.Models;

namespace WebKontorExpert.Components
{
    public class CategoryListSmallViewComponent : ViewComponent
    {
        private readonly IParentCategoryData _parentCategoryData;
        private readonly ICategoryData _categoryData;

        public CategoryListSmallViewComponent(IParentCategoryData parentCategoryData, ICategoryData categoryData)
        {
            _parentCategoryData = parentCategoryData;
            _categoryData = categoryData;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var parentCategories = await _parentCategoryData.GetAllParentCategories();

            foreach (var parentCategory in parentCategories)
            {
                parentCategory.Categories = await _categoryData.GetCategoriesByParentCategoryId(parentCategory.ParentCategoryID);
            }

            var viewModel = new CategoryListModelView
            {
                ParentCategories = parentCategories
            };

            // Transform the data into a dictionary for easy access
            var categoriesGrouped = new Dictionary<string, List<ParentCategory>>
            {
                { "Bord", parentCategories.Where(pc => new[] { 13, 14, 15 }.Contains(pc.ParentCategoryID)).ToList() },
                { "Stol", parentCategories.Where(pc => new[] { 16, 17 }.Contains(pc.ParentCategoryID)).ToList() },
                { "Opbevaring", parentCategories.Where(pc => new[] { 18, 19, 20, 21 }.Contains(pc.ParentCategoryID)).ToList() },
                { "AfskærmingOgLyddæmpning", parentCategories.Where(pc => new[] { 22, 23, 24 }.Contains(pc.ParentCategoryID)).ToList() },
                { "Belysning", parentCategories.Where(pc => pc.ParentCategoryID == 25).ToList() },
                { "Tavler", parentCategories.Where(pc => pc.ParentCategoryID == 26).ToList() },
                { "SofaerOgLænestole", parentCategories.Where(pc => new[] { 27, 28, 29 }.Contains(pc.ParentCategoryID)).ToList() }
            };

            ViewBag.CategoriesGrouped = categoriesGrouped;

            return View(viewModel);
        }
    }
}
