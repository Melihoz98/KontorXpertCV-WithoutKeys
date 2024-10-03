using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using WebKontorExpert.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebKontorExpert.DataAccess
{
    public class ProductImageAccess : IProductImageAccess
    {
        private readonly string _connectionString;

        public ProductImageAccess(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                                ?? throw new InvalidOperationException("Database connection string is not configured.");
        }

        public async Task<List<ProductImage>> GetAllProductImages()
        {
            var images = new List<ProductImage>();

            try
            {
                const string queryString = "SELECT ImageID, ProductID, ImageUrl FROM ProductImages";

                using (var con = new SqlConnection(_connectionString))
                using (var readCommand = new SqlCommand(queryString, con))
                {
                    await con.OpenAsync();

                    using (var reader = await readCommand.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var image = GetProductImageFromReader(reader);
                            images.Add(image);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving product images: {ex.Message}");
                throw;
            }

            return images;
        }

        public async Task<List<ProductImage>> GetProductImagesByProductId(int productId)
        {
            var images = new List<ProductImage>();

            try
            {
                const string queryString = "SELECT ImageID, ProductID, ImageUrl FROM ProductImages WHERE ProductID = @ProductID";

                using (var con = new SqlConnection(_connectionString))
                using (var command = new SqlCommand(queryString, con))
                {
                    command.Parameters.AddWithValue("@ProductID", productId);

                    await con.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var image = GetProductImageFromReader(reader);
                            images.Add(image);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving product images by product ID: {ex.Message}");
                throw;
            }

            return images;
        }

        public async Task<ProductImage> GetProductImageById(int imageId)
        {
            ProductImage foundImage = null;

            try
            {
                const string queryString = "SELECT ImageID, ProductID, ImageUrl FROM ProductImages WHERE ImageID = @ImageID";

                using (var con = new SqlConnection(_connectionString))
                using (var readCommand = new SqlCommand(queryString, con))
                {
                    readCommand.Parameters.AddWithValue("@ImageID", imageId);

                    await con.OpenAsync();

                    using (var reader = await readCommand.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            foundImage = GetProductImageFromReader(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving product image: {ex.Message}");
                throw;
            }

            return foundImage;
        }

        public async Task<int> AddProductImage(ProductImage productImage)
        {
            int insertedId = -1;

            try
            {
                const string insertString = "INSERT INTO ProductImages (ProductID, ImageUrl) OUTPUT INSERTED.ImageID VALUES (@ProductID, @ImageUrl)";

                using (var con = new SqlConnection(_connectionString))
                using (var createCommand = new SqlCommand(insertString, con))
                {
                    createCommand.Parameters.AddWithValue("@ProductID", productImage.ProductID);
                    createCommand.Parameters.AddWithValue("@ImageUrl", productImage.ImageUrl);

                    await con.OpenAsync();
                    insertedId = (int)await createCommand.ExecuteScalarAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding product image: {ex.Message}");
                throw;
            }

            return insertedId;
        }

        public async Task UpdateProductImage(ProductImage productImage)
        {
            try
            {
                const string updateString = "UPDATE ProductImages SET ProductID = @ProductID, ImageUrl = @ImageUrl WHERE ImageID = @ImageID";

                using (var con = new SqlConnection(_connectionString))
                using (var updateCommand = new SqlCommand(updateString, con))
                {
                    updateCommand.Parameters.AddWithValue("@ProductID", productImage.ProductID);
                    updateCommand.Parameters.AddWithValue("@ImageUrl", productImage.ImageUrl);
                    updateCommand.Parameters.AddWithValue("@ImageID", productImage.ImageID);

                    await con.OpenAsync();
                    await updateCommand.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating product image: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteProductImage(int imageId)
        {
            try
            {
                const string deleteString = "DELETE FROM ProductImages WHERE ImageID = @ImageID";

                using (var con = new SqlConnection(_connectionString))
                using (var deleteCommand = new SqlCommand(deleteString, con))
                {
                    deleteCommand.Parameters.AddWithValue("@ImageID", imageId);

                    await con.OpenAsync();
                    await deleteCommand.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting product image: {ex.Message}");
                throw;
            }
        }

        private ProductImage GetProductImageFromReader(SqlDataReader reader)
        {
            return new ProductImage
            {
                ImageID = reader.GetInt32(reader.GetOrdinal("ImageID")),
                ProductID = reader.GetInt32(reader.GetOrdinal("ProductID")),
                ImageUrl = reader.GetString(reader.GetOrdinal("ImageUrl"))
            };
        }
    }
}
