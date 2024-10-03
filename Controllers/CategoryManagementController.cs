using Microsoft.AspNetCore.Mvc;
using WebKontorExpert.BusinessLogic;
using WebKontorExpert.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebKontorExpert.Controllers
{
    public class CategoryManagementController : Controller
    {
        private readonly ICategoryData _categoryData;
        private readonly IParentCategoryData _parentCategoryData;

        public CategoryManagementController(ICategoryData categoryData, IParentCategoryData parentCategoryData)
        {
            _categoryData = categoryData;
            _parentCategoryData = parentCategoryData;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new CategoryManagementViewModel
            {
                ParentCategories = await _parentCategoryData.GetAllParentCategories(),
                Categories = await _categoryData.GetAllCategories()
            };

            return View("/Views/Management/CategoryManagement/Index.cshtml", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(Category newCategory)
        {
            if (!ModelState.IsValid)
            {
                await _categoryData.AddCategory(newCategory);
                return RedirectToAction(nameof(Index));
            }
            return View(newCategory);
        }

        [HttpPost]
        public async Task<IActionResult> CreateParentCategory(ParentCategory newParentCategory)
        {
            if (!ModelState.IsValid)
            {
                await _parentCategoryData.AddParentCategory(newParentCategory);
                return RedirectToAction(nameof(Index));
            }
            return View(newParentCategory);
        }

        [HttpPost]
        public async Task<IActionResult> EditCategory(Category category)
        {
            if (!ModelState.IsValid)
            {
                await _categoryData.UpdateCategory(category);
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> EditParentCategory(ParentCategory parentCategory)
        {
            if (!ModelState.IsValid)
            {
                await _parentCategoryData.UpdateParentCategory(parentCategory);
                return RedirectToAction(nameof(Index));
            }
            return View(parentCategory);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategory(int categoryId)
        {
            await _categoryData.DeleteCategory(categoryId);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteParentCategory(int parentCategoryId)
        {
            await _parentCategoryData.DeleteParentCategory(parentCategoryId);
            return RedirectToAction(nameof(Index));
        }
    }
}
