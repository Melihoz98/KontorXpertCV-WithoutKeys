using WebKontorExpert.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebKontorExpert.DataAccess
{
    public interface IProductImageAccess
    {
        Task<List<ProductImage>> GetAllProductImages();
        Task<List<ProductImage>> GetProductImagesByProductId(int productId);
        Task<ProductImage> GetProductImageById(int imageId);
        Task<int> AddProductImage(ProductImage productImage);
        Task UpdateProductImage(ProductImage productImage);
        Task DeleteProductImage(int imageId);
    }
}
