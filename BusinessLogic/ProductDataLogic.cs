using WebKontorExpert.DataAccess;
using WebKontorExpert.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace WebKontorExpert.BusinessLogic
{
    public class ProductDataLogic : IProductData
    {
        private readonly IProductAccess _productAccess;

        public ProductDataLogic(IProductAccess productAccess)
        {
            _productAccess = productAccess;
        }

        public async Task<List<Product>> GetAllProducts()
        {
            return await _productAccess.GetAllProducts();
        }

        public async Task<List<Product>> GetUsedProducts()
        {
            return await _productAccess.GetUsedProducts();
        }

        public async Task<List<Product>> GetNewProducts()
        {
            return await _productAccess.GetNewProducts();
        }

        public async Task<Product> GetProductById(int productId)
        {
            return await _productAccess.GetProductById(productId);
        }

        public async Task<Product> GetProductByName(string productName)
        {
            return await _productAccess.GetProductByName(productName);
        }

        public async Task<int> AddProduct(Product product, List<string> imageUrls)
        {
            return await _productAccess.AddProduct(product, imageUrls);
        }

        public async Task UpdateProduct(Product product)
        {
            await _productAccess.UpdateProduct(product);
        }

        public async Task DeleteProduct(int productId)
        {
            await _productAccess.DeleteProduct(productId);
        }

        public async Task<List<Product>> GetProductsByCategoryID(int categoryID)
        {
            return await _productAccess.GetProductsByCategoryID(categoryID);
        }

        public async Task<List<Product>> GetProductsByParentCategoryId(int parentCategoryId)
        {
            return await _productAccess.GetProductsByParentCategoryId(parentCategoryId);
        }

        public async Task AddProductImages(int productId, List<string> imageUrls)
        {
            await _productAccess.AddProductImages(productId, imageUrls);
        }

        public async Task DeleteProductImages(int productId)
        {
            await _productAccess.DeleteProductImages(productId);
        }

        public async Task DeleteProductImage(int imageId)
        {
            var imageUrl = await _productAccess.GetProductImageUrlById(imageId); // Assuming you have a method to retrieve image URL by image ID
            if (imageUrl != null)
            {
                // Delete from wwwroot
                string imagePath = Path.Combine("wwwroot", imageUrl.TrimStart('/'));
                if (File.Exists(imagePath))
                {
                    File.Delete(imagePath);
                }

                // Delete from database
                await _productAccess.DeleteProductImage(imageId);
            }
        }

        public async Task<List<Product>> SearchProducts(string searchTerm)
        {
            return await _productAccess.SearchProducts(searchTerm);
        }

        // Implement the new method here
        public async Task<List<Product>> GetLatestProducts(int topN)
        {
            return await _productAccess.GetLatestProducts(topN);
        }
    }
}
