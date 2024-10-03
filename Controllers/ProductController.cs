using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebKontorExpert.BusinessLogic;
using WebKontorExpert.Models;

public class ProductController : Controller
{
    private readonly IProductData _productData;
    private readonly ICategoryData _categoryData;

    public ProductController(IProductData productData, ICategoryData categoryData)
    {
        _productData = productData;
        _categoryData = categoryData;
    }

    public async Task<IActionResult> Index(int? categoryID, int? parentCategoryID, string searchTerm)
    {
        List<Product> products;

        if (!string.IsNullOrEmpty(searchTerm))
        {
            products = await _productData.SearchProducts(searchTerm);
            ViewBag.SearchTerm = searchTerm;
            ViewBag.ProductCount = products.Count;
        }
        else if (categoryID.HasValue)
        {
            products = await _productData.GetProductsByCategoryID(categoryID.Value);
            var category = await _categoryData.GetCategoryById(categoryID.Value);
            ViewBag.CategoryName = category?.CategoryName;
            ViewBag.ProductCount = products.Count;
        }
        else if (parentCategoryID.HasValue)
        {
            products = await _productData.GetProductsByParentCategoryId(parentCategoryID.Value);
            ViewBag.ParentCategoryID = parentCategoryID;
            ViewBag.ProductCount = products.Count;
        }
        else
        {
            products = new List<Product>();
            ViewBag.ProductCount = 0;
        }

        if (products.Any())
        {
            // Calculate min and max prices
            var minPrice = products.Min(p => p.DiscountPriceWithoutVAT.HasValue && p.DiscountPriceWithoutVAT.Value > p.PriceWithoutVAT
                                             ? p.PriceWithoutVAT : p.DiscountPriceWithoutVAT ?? p.PriceWithoutVAT);

            var maxPrice = products.Max(p => p.DiscountPriceWithoutVAT.HasValue && p.DiscountPriceWithoutVAT.Value > p.PriceWithoutVAT
                                             ? p.DiscountPriceWithoutVAT.Value : p.PriceWithoutVAT);

            // Calculate min and max stock quantity
            var minAntal = products.Min(p => p.StockQuantity);
            var maxAntal = products.Max(p => p.StockQuantity);

            // Get distinct brands, colors, and conditions
            var brands = products.Select(p => p.Brand).Distinct().ToList();
            var colors = products.Select(p => p.Color).Distinct().ToList();
            var conditions = products.Select(p => p.IsUsed.HasValue && p.IsUsed.Value ? "Brugt" : "Ny").Distinct().ToList();

            // Pass the data to the view
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;
            ViewBag.MinAntal = minAntal;
            ViewBag.MaxAntal = maxAntal;
            ViewBag.Brands = brands;
            ViewBag.Colors = colors;
            ViewBag.Conditions = conditions;
        }
        else
        {
            // Set defaults if no products are found
            ViewBag.MinPrice = 0;
            ViewBag.MaxPrice = 0;
            ViewBag.MinAntal = 0;
            ViewBag.MaxAntal = 0;
            ViewBag.Brands = new List<string>();
            ViewBag.Colors = new List<string>();
            ViewBag.Conditions = new List<string>();
        }


        return View(products);
    }

    public async Task<IActionResult> ProductPage(int id)
    {
        var product = await _productData.GetProductById(id);
        if (product == null)
        {
            return NotFound();
        }
        return View(product);
    }
}
