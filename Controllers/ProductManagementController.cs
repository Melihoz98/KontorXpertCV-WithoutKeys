using Microsoft.AspNetCore.Mvc;
using WebKontorExpert.BusinessLogic;
using WebKontorExpert.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace WebKontorExpert.Controllers
{
    public class ProductManagementController : Controller
    {
        private readonly IProductData _productData;
        private readonly ICategoryData _categoryData;

        public ProductManagementController(IProductData productData, ICategoryData categoryData)
        {
            _productData = productData;
            _categoryData = categoryData;
        }

        public async Task<IActionResult> Index()
        {
            List<Product> products = await _productData.GetAllProducts();
            ViewBag.Categories = await _categoryData.GetAllCategories(); // Hent alle kategorier
            return View("~/Views/Management/ProductManagement/Index.cshtml", products);
        }

        public async Task<IActionResult> Details(int id)
        {
            Product product = await _productData.GetProductById(id);
            if (product == null)
            {
                return NotFound();
            }
            return View("~/Views/Management/ProductManagement/Details.cshtml", product);
        }

        public IActionResult Create()
        {
            return View("~/Views/Management/ProductManagement/Create.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product product, IFormFile MainImage, List<IFormFile> ImageFiles)
        {
            if (!ModelState.IsValid)
            {
                var imageUrls = new List<string>();

                if (MainImage != null && MainImage.Length > 0)
                {
                    var mainImagePath = Path.Combine("wwwroot/images/ProductImages", MainImage.FileName);
                    using (var stream = new FileStream(mainImagePath, FileMode.Create))
                    {
                        await MainImage.CopyToAsync(stream);
                    }
                    product.MainImageUrl = $"/images/ProductImages/{MainImage.FileName}";
                }

                if (ImageFiles != null && ImageFiles.Count > 0)
                {
                    foreach (var image in ImageFiles)
                    {
                        if (image.Length > 0)
                        {
                            var filePath = Path.Combine("wwwroot/images/ProductImages", image.FileName);
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await image.CopyToAsync(stream);
                            }
                            imageUrls.Add($"/images/ProductImages/{image.FileName}");
                        }
                    }
                }

                await _productData.AddProduct(product, imageUrls);
                return RedirectToAction(nameof(Index));
            }

            foreach (var modelStateEntry in ModelState.Values)
            {
                foreach (var error in modelStateEntry.Errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }

            return View("~/Views/Management/ProductManagement/Create.cshtml", product);
        }

        public async Task<IActionResult> Edit(int id)
        {
            Product product = await _productData.GetProductById(id);
            if (product == null)
            {
                return NotFound();
            }

            ViewBag.Categories = await _categoryData.GetAllCategories(); // Hent alle kategorier

            return View("~/Views/Management/ProductManagement/Edit.cshtml", product);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(Product product, IFormFile MainImage, List<IFormFile> ImageFiles, List<int> DeleteImages)
        {
            if (!ModelState.IsValid)
            {
                // Handling Main Image update
                if (MainImage != null && MainImage.Length > 0)
                {
                    var mainImagePath = Path.Combine("wwwroot/images/ProductImages", MainImage.FileName);
                    using (var stream = new FileStream(mainImagePath, FileMode.Create))
                    {
                        await MainImage.CopyToAsync(stream);
                    }
                    product.MainImageUrl = $"/images/ProductImages/{MainImage.FileName}";
                }

                // Update product details (excluding images) in the database
                await _productData.UpdateProduct(product);

                // Handling Additional Images update
                if (ImageFiles != null && ImageFiles.Count > 0)
                {
                    var imageUrls = new List<string>();
                    foreach (var image in ImageFiles)
                    {
                        if (image.Length > 0)
                        {
                            var filePath = Path.Combine("wwwroot/images/ProductImages", image.FileName);
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await image.CopyToAsync(stream);
                            }
                            imageUrls.Add($"/images/ProductImages/{image.FileName}");
                        }
                    }

                    // Save new images to ProductImages table
                    await _productData.AddProductImages(product.ProductID, imageUrls);
                }

                // Handling deletion of images
                if (DeleteImages != null && DeleteImages.Any())
                {
                    foreach (var imageId in DeleteImages)
                    {
                        // Implement logic to delete image from ProductImages table and wwwroot
                        await _productData.DeleteProductImage(imageId);
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            // If ModelState is not valid, return the edit view with the product model
            return View("~/Views/Management/ProductManagement/Edit.cshtml", product);
        }

        public async Task<IActionResult> Delete(int id)
        {
            Product product = await _productData.GetProductById(id);
            if (product == null)
            {
                return NotFound();
            }
            return View("~/Views/Management/ProductManagement/Delete.cshtml", product);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _productData.DeleteProduct(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
