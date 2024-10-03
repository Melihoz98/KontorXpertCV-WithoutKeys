using WebKontorExpert.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebKontorExpert.BusinessLogic
{
    public interface IProductData
    {
        Task<List<Product>> GetAllProducts();
        Task<List<Product>> GetUsedProducts();
        Task<List<Product>> GetProductsByParentCategoryId(int parentCategoryId);
        Task<List<Product>> GetProductsByCategoryID(int categoryID);
        Task<List<Product>> GetNewProducts();
        Task<Product> GetProductById(int productId);
        Task<Product> GetProductByName(string productName);
        Task<int> AddProduct(Product product, List<string> imageUrls);
        Task UpdateProduct(Product product);
        Task DeleteProduct(int productId);
        Task AddProductImages(int productId, List<string> imageUrls);
        Task DeleteProductImages(int productId);
        Task DeleteProductImage(int imageId);
        Task<List<Product>> SearchProducts(string searchTerm);

        // Add the new method signature here
        Task<List<Product>> GetLatestProducts(int topN);
    }
}
