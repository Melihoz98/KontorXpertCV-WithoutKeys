using WebKontorExpert.DataAccess;
using WebKontorExpert.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebKontorExpert.BusinessLogic;

namespace WebKontorExpert.BusinessLogic
{
    public class ProductImageDataLogic : IProductImageData
    {
        private readonly IProductImageAccess _productImageAccess;

        public ProductImageDataLogic(IProductImageAccess productImageAccess)
        {
            _productImageAccess = productImageAccess;
        }

        public async Task<List<ProductImage>> GetAllProductImages()
        {
            return await _productImageAccess.GetAllProductImages();
        }

        public async Task<List<ProductImage>> GetProductImagesByProductId(int productId)
        {
            return await _productImageAccess.GetProductImagesByProductId(productId);
        }

        public async Task<ProductImage> GetProductImageById(int imageId)
        {
            return await _productImageAccess.GetProductImageById(imageId);
        }

        public async Task<int> AddProductImage(ProductImage productImage)
        {
            return await _productImageAccess.AddProductImage(productImage);
        }

        public async Task UpdateProductImage(ProductImage productImage)
        {
            await _productImageAccess.UpdateProductImage(productImage);
        }

        public async Task DeleteProductImage(int imageId)
        {
            await _productImageAccess.DeleteProductImage(imageId);
        }
    }
}
