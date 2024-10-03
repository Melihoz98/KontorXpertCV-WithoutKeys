using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using WebKontorExpert.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Stripe.Terminal;
using System.Reflection.PortableExecutable;

namespace WebKontorExpert.DataAccess
{
    public class ProductAccess : IProductAccess
    {
        private readonly string _connectionString;

        public ProductAccess(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                                ?? throw new InvalidOperationException("Database connection string is not configured.");
        }

        public async Task<List<Product>> GetAllProducts()
        {
            var products = new List<Product>();

            try
            {
                const string queryString = @"
            SELECT p.ProductID, p.SKU, p.Name, p.Description, p.Brand, p.Price, p.StockQuantity, p.Color, p.Dimensions, p.CategoryID, p.IsUsed, p.MainImageUrl, p.Discount,
                   pi.ImageID, pi.ImageUrl
            FROM Products p
            LEFT JOIN ProductImages pi ON p.ProductID = pi.ProductID";

                using (var con = new SqlConnection(_connectionString))
                using (var readCommand = new SqlCommand(queryString, con))
                {
                    await con.OpenAsync();

                    using (var productReader = await readCommand.ExecuteReaderAsync())
                    {
                        while (await productReader.ReadAsync())
                        {
                            var productId = productReader.GetInt32(productReader.GetOrdinal("ProductID"));
                            var existingProduct = products.Find(p => p.ProductID == productId);

                            if (existingProduct == null)
                            {
                                var newProduct = GetProductFromReader(productReader);
                                products.Add(newProduct);
                            }

                            // Add ProductImage if exists
                            if (!productReader.IsDBNull(productReader.GetOrdinal("ImageID")))
                            {
                                var productImage = new ProductImage
                                {
                                    ImageID = productReader.GetInt32(productReader.GetOrdinal("ImageID")),
                                    ProductID = productId,
                                    ImageUrl = productReader.GetString(productReader.GetOrdinal("ImageUrl"))
                                };

                                var productToAddImage = products.Find(p => p.ProductID == productId);
                                if (productToAddImage != null)
                                {
                                    productToAddImage.Images ??= new List<ProductImage>();
                                    productToAddImage.Images.Add(productImage);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving products: {ex.Message}");
                throw;
            }

            return products;
        }





        public async Task<List<Product>> GetLatestProducts(int count)
        {
            var products = new List<Product>();

            try
            {
                const string queryString = @"
            SELECT TOP (@Count) p.ProductID, p.SKU, p.Name, p.Description, p.Brand, p.Price, p.StockQuantity, p.Color, p.Dimensions, p.CategoryID, p.IsUsed, p.MainImageUrl, p.Discount
            FROM Products p
            ORDER BY p.ProductID DESC";

                using (var con = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand(queryString, con))
                {
                    cmd.Parameters.AddWithValue("@Count", count);

                    await con.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var product = new Product
                            {
                                ProductID = reader.GetInt32(reader.GetOrdinal("ProductID")),
                                SKU = reader.IsDBNull(reader.GetOrdinal("SKU")) ? (short?)null : reader.GetInt16(reader.GetOrdinal("SKU")),
                                Name = reader.IsDBNull(reader.GetOrdinal("Name")) ? null : reader.GetString(reader.GetOrdinal("Name")),
                                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                                Brand = reader.IsDBNull(reader.GetOrdinal("Brand")) ? null : reader.GetString(reader.GetOrdinal("Brand")),
                                Price = reader.IsDBNull(reader.GetOrdinal("Price")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("Price")),
                                StockQuantity = reader.IsDBNull(reader.GetOrdinal("StockQuantity")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("StockQuantity")),
                                Color = reader.IsDBNull(reader.GetOrdinal("Color")) ? null : reader.GetString(reader.GetOrdinal("Color")),
                                Dimensions = reader.IsDBNull(reader.GetOrdinal("Dimensions")) ? null : reader.GetString(reader.GetOrdinal("Dimensions")),
                                CategoryID = reader.IsDBNull(reader.GetOrdinal("CategoryID")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("CategoryID")),
                                IsUsed = reader.IsDBNull(reader.GetOrdinal("IsUsed")) ? (bool?)null : reader.GetBoolean(reader.GetOrdinal("IsUsed")),
                                MainImageUrl = reader.IsDBNull(reader.GetOrdinal("MainImageUrl")) ? null : reader.GetString(reader.GetOrdinal("MainImageUrl")),
                                Discount = reader.IsDBNull(reader.GetOrdinal("Discount")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("Discount"))
                            };

                            products.Add(product);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving latest products: {ex.Message}");
                throw;
            }

            return products;
        }



        public async Task<List<Product>> GetProductsByCategoryID(int categoryID)
        {
            var products = new List<Product>();

            try
            {
                const string queryString = @"
            SELECT p.ProductID, p.SKU, p.Name, p.Description, p.Brand, p.Price, p.StockQuantity, p.Color, p.Dimensions, p.CategoryID, p.IsUsed, p.MainImageUrl, p.Discount,
                   pi.ImageID, pi.ImageUrl
            FROM Products p
            LEFT JOIN ProductImages pi ON p.ProductID = pi.ProductID
            WHERE p.CategoryID = @CategoryID";

                using (var con = new SqlConnection(_connectionString))
                using (var command = new SqlCommand(queryString, con))
                {
                    command.Parameters.AddWithValue("@CategoryID", categoryID);

                    await con.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var productId = reader.GetInt32(reader.GetOrdinal("ProductID"));
                            var existingProduct = products.Find(p => p.ProductID == productId);

                            if (existingProduct == null)
                            {
                                var newProduct = GetProductFromReader(reader);
                                products.Add(newProduct);
                            }

                            // Add ProductImage if exists
                            if (!reader.IsDBNull(reader.GetOrdinal("ImageID")))
                            {
                                var productImage = new ProductImage
                                {
                                    ImageID = reader.GetInt32(reader.GetOrdinal("ImageID")),
                                    ProductID = productId,
                                    ImageUrl = reader.GetString(reader.GetOrdinal("ImageUrl"))
                                };

                                var productToAddImage = products.Find(p => p.ProductID == productId);
                                if (productToAddImage != null)
                                {
                                    productToAddImage.Images ??= new List<ProductImage>();
                                    productToAddImage.Images.Add(productImage);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving products by category ID: {ex.Message}");
                throw;
            }

            return products;
        }

        public async Task<List<Product>> GetUsedProducts()
        {
            return await GetProductsByUsageStatus(true);
        }

        public async Task<List<Product>> GetNewProducts()
        {
            return await GetProductsByUsageStatus(false);
        }

        private async Task<List<Product>> GetProductsByUsageStatus(bool isUsed)
        {
            var filteredProducts = new List<Product>();

            try
            {
                const string queryString = @"
            SELECT p.ProductID, p.SKU, p.Name, p.Description, p.Brand, p.Price, p.StockQuantity, p.Color, p.Dimensions, p.CategoryID, p.IsUsed, p.MainImageUrl, p.Discount,
                   pi.ImageID, pi.ImageUrl
            FROM Products p
            LEFT JOIN ProductImages pi ON p.ProductID = pi.ProductID
            WHERE p.IsUsed = @IsUsed";

                using (var con = new SqlConnection(_connectionString))
                using (var command = new SqlCommand(queryString, con))
                {
                    command.Parameters.AddWithValue("@IsUsed", isUsed);

                    await con.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var productId = reader.GetInt32(reader.GetOrdinal("ProductID"));
                            var existingProduct = filteredProducts.Find(p => p.ProductID == productId);

                            if (existingProduct == null)
                            {
                                var newProduct = new Product
                                {
                                    ProductID = reader.GetInt32(reader.GetOrdinal("ProductID")),
                                    SKU = reader.IsDBNull(reader.GetOrdinal("SKU")) ? (short?)null : reader.GetInt16(reader.GetOrdinal("SKU")),
                                    Name = reader.IsDBNull(reader.GetOrdinal("Name")) ? null : reader.GetString(reader.GetOrdinal("Name")),
                                    Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                                    Brand = reader.IsDBNull(reader.GetOrdinal("Brand")) ? null : reader.GetString(reader.GetOrdinal("Brand")),
                                    Price = reader.IsDBNull(reader.GetOrdinal("Price")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("Price")),
                                    StockQuantity = reader.IsDBNull(reader.GetOrdinal("StockQuantity")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("StockQuantity")),
                                    Color = reader.IsDBNull(reader.GetOrdinal("Color")) ? null : reader.GetString(reader.GetOrdinal("Color")),
                                    Dimensions = reader.IsDBNull(reader.GetOrdinal("Dimensions")) ? null : reader.GetString(reader.GetOrdinal("Dimensions")),
                                    CategoryID = reader.IsDBNull(reader.GetOrdinal("CategoryID")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("CategoryID")),
                                    IsUsed = reader.IsDBNull(reader.GetOrdinal("IsUsed")) ? (bool?)null : reader.GetBoolean(reader.GetOrdinal("IsUsed")),
                                    MainImageUrl = reader.IsDBNull(reader.GetOrdinal("MainImageUrl")) ? null : reader.GetString(reader.GetOrdinal("MainImageUrl")),
                                    Discount = reader.IsDBNull(reader.GetOrdinal("Discount")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("Discount"))
                                };

                                filteredProducts.Add(newProduct);
                            }

                            // Add ProductImage if exists
                            if (!reader.IsDBNull(reader.GetOrdinal("ImageID")))
                            {
                                var productImage = new ProductImage
                                {
                                    ImageID = reader.GetInt32(reader.GetOrdinal("ImageID")),
                                    ProductID = productId,
                                    ImageUrl = reader.GetString(reader.GetOrdinal("ImageUrl"))
                                };

                                var productToAddImage = filteredProducts.Find(p => p.ProductID == productId);
                                if (productToAddImage != null)
                                {
                                    productToAddImage.Images ??= new List<ProductImage>();
                                    productToAddImage.Images.Add(productImage);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving products by usage status: {ex.Message}");
                throw;
            }

            return filteredProducts;
        }


        public async Task<Product> GetProductById(int productId)
        {
            Product foundProduct = null;

            try
            {
                const string queryString = @"
            SELECT p.ProductID, p.SKU, p.Name, p.Description, p.Brand, p.Price, p.StockQuantity, p.Color, p.Dimensions, p.CategoryID, p.IsUsed, p.MainImageUrl, p.Discount,
                   pi.ImageID, pi.ImageUrl
            FROM Products p
            LEFT JOIN ProductImages pi ON p.ProductID = pi.ProductID
            WHERE p.ProductID = @ProductId";

                using (var con = new SqlConnection(_connectionString))
                using (var command = new SqlCommand(queryString, con))
                {
                    command.Parameters.AddWithValue("@ProductId", productId);

                    await con.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            if (foundProduct == null)
                            {
                                foundProduct = new Product
                                {
                                    ProductID = reader.GetInt32(reader.GetOrdinal("ProductID")),
                                    SKU = reader.IsDBNull(reader.GetOrdinal("SKU")) ? (short?)null : reader.GetInt16(reader.GetOrdinal("SKU")),
                                    Name = reader.IsDBNull(reader.GetOrdinal("Name")) ? null : reader.GetString(reader.GetOrdinal("Name")),
                                    Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                                    Brand = reader.IsDBNull(reader.GetOrdinal("Brand")) ? null : reader.GetString(reader.GetOrdinal("Brand")),
                                    Price = reader.IsDBNull(reader.GetOrdinal("Price")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("Price")),
                                    StockQuantity = reader.IsDBNull(reader.GetOrdinal("StockQuantity")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("StockQuantity")),
                                    Color = reader.IsDBNull(reader.GetOrdinal("Color")) ? null : reader.GetString(reader.GetOrdinal("Color")),
                                    Dimensions = reader.IsDBNull(reader.GetOrdinal("Dimensions")) ? null : reader.GetString(reader.GetOrdinal("Dimensions")),
                                    CategoryID = reader.IsDBNull(reader.GetOrdinal("CategoryID")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("CategoryID")),
                                    IsUsed = reader.IsDBNull(reader.GetOrdinal("IsUsed")) ? (bool?)null : reader.GetBoolean(reader.GetOrdinal("IsUsed")),
                                    MainImageUrl = reader.IsDBNull(reader.GetOrdinal("MainImageUrl")) ? null : reader.GetString(reader.GetOrdinal("MainImageUrl")),
                                    Discount = reader.IsDBNull(reader.GetOrdinal("Discount")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("Discount"))
                                };
                            }

                            // Add ProductImage if exists
                            if (!reader.IsDBNull(reader.GetOrdinal("ImageID")))
                            {
                                foundProduct.Images ??= new List<ProductImage>();
                                foundProduct.Images.Add(new ProductImage
                                {
                                    ImageID = reader.GetInt32(reader.GetOrdinal("ImageID")),
                                    ProductID = productId,
                                    ImageUrl = reader.GetString(reader.GetOrdinal("ImageUrl"))
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving product by ID: {ex.Message}");
                throw;
            }

            return foundProduct;
        }


        public async Task<Product> GetProductByName(string productName)
        {
            Product product = null;

            try
            {
                const string queryString = @"
            SELECT p.ProductID, p.SKU, p.Name, p.Description, p.Brand, p.Price, p.StockQuantity, p.Color, p.Dimensions, p.CategoryID, p.IsUsed, p.MainImageUrl, p.Discount,
                   pi.ImageID, pi.ImageUrl
            FROM Products p
            LEFT JOIN ProductImages pi ON p.ProductID = pi.ProductID
            WHERE p.Name = @ProductName";

                using (var con = new SqlConnection(_connectionString))
                using (var command = new SqlCommand(queryString, con))
                {
                    command.Parameters.AddWithValue("@ProductName", productName);

                    await con.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            product = new Product
                            {
                                ProductID = reader.GetInt32(reader.GetOrdinal("ProductID")),
                                SKU = reader.IsDBNull(reader.GetOrdinal("SKU")) ? (short?)null : reader.GetInt16(reader.GetOrdinal("SKU")),
                                Name = reader.IsDBNull(reader.GetOrdinal("Name")) ? null : reader.GetString(reader.GetOrdinal("Name")),
                                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                                Brand = reader.IsDBNull(reader.GetOrdinal("Brand")) ? null : reader.GetString(reader.GetOrdinal("Brand")),
                                Price = reader.IsDBNull(reader.GetOrdinal("Price")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("Price")),
                                StockQuantity = reader.IsDBNull(reader.GetOrdinal("StockQuantity")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("StockQuantity")),
                                Color = reader.IsDBNull(reader.GetOrdinal("Color")) ? null : reader.GetString(reader.GetOrdinal("Color")),
                                Dimensions = reader.IsDBNull(reader.GetOrdinal("Dimensions")) ? null : reader.GetString(reader.GetOrdinal("Dimensions")),
                                CategoryID = reader.IsDBNull(reader.GetOrdinal("CategoryID")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("CategoryID")),
                                IsUsed = reader.IsDBNull(reader.GetOrdinal("IsUsed")) ? (bool?)null : reader.GetBoolean(reader.GetOrdinal("IsUsed")),
                                MainImageUrl = reader.IsDBNull(reader.GetOrdinal("MainImageUrl")) ? null : reader.GetString(reader.GetOrdinal("MainImageUrl")),
                                Discount = reader.IsDBNull(reader.GetOrdinal("Discount")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("Discount"))
                            };

                            // Add ProductImage if exists
                            if (!reader.IsDBNull(reader.GetOrdinal("ImageID")))
                            {
                                product.Images ??= new List<ProductImage>();
                                product.Images.Add(new ProductImage
                                {
                                    ImageID = reader.GetInt32(reader.GetOrdinal("ImageID")),
                                    ProductID = product.ProductID,
                                    ImageUrl = reader.GetString(reader.GetOrdinal("ImageUrl"))
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving product by name: {ex.Message}");
                throw;
            }

            return product;
        }


        public async Task<int> AddProduct(Product product, List<string> imageUrls)
        {
            int insertedProductId = -1;

            try
            {
                const string insertProductString = @"
            INSERT INTO Products (Name, SKU, Description, Brand, Price, StockQuantity, Color, Dimensions, CategoryID, IsUsed, MainImageUrl, Discount)
            OUTPUT INSERTED.ProductID
            VALUES (@Name, @SKU, @Description, @Brand, @Price, @StockQuantity, @Color, @Dimensions, @CategoryID, @IsUsed, @MainImageUrl, @Discount)";

                using (var con = new SqlConnection(_connectionString))
                using (var createProductCommand = new SqlCommand(insertProductString, con))
                {
                    createProductCommand.Parameters.AddWithValue("@Name", product.Name);
                    createProductCommand.Parameters.AddWithValue("@SKU", product.SKU);
                    createProductCommand.Parameters.AddWithValue("@Description", product.Description);
                    createProductCommand.Parameters.AddWithValue("@Brand", product.Brand);
                    createProductCommand.Parameters.AddWithValue("@Price", product.Price);
                    createProductCommand.Parameters.AddWithValue("@StockQuantity", product.StockQuantity);
                    createProductCommand.Parameters.AddWithValue("@Color", product.Color);
                    createProductCommand.Parameters.AddWithValue("@Dimensions", product.Dimensions);
                    createProductCommand.Parameters.AddWithValue("@CategoryID", product.CategoryID);
                    createProductCommand.Parameters.AddWithValue("@IsUsed", product.IsUsed);
                    createProductCommand.Parameters.AddWithValue("@MainImageUrl", product.MainImageUrl);
                    createProductCommand.Parameters.AddWithValue("@Discount", product.Discount ?? (object)DBNull.Value);

                    await con.OpenAsync();
                    insertedProductId = (int)await createProductCommand.ExecuteScalarAsync();
                }

                // Insert product images
                if (insertedProductId > 0 && imageUrls != null && imageUrls.Count > 0)
                {
                    const string insertImageString = @"
                INSERT INTO ProductImages (ProductID, ImageUrl)
                VALUES (@ProductID, @ImageUrl)";

                    using (var con = new SqlConnection(_connectionString))
                    {
                        await con.OpenAsync();

                        foreach (var imageUrl in imageUrls)
                        {
                            using (var createImageCommand = new SqlCommand(insertImageString, con))
                            {
                                createImageCommand.Parameters.AddWithValue("@ProductID", insertedProductId);
                                createImageCommand.Parameters.AddWithValue("@ImageUrl", imageUrl);

                                await createImageCommand.ExecuteNonQueryAsync();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding product: {ex.Message}");
                throw;
            }

            return insertedProductId;
        }



        public async Task UpdateProduct(Product product)
        {
            try
            {
                const string updateString = @"
            UPDATE Products 
            SET Name = @Name, SKU = @SKU, Description = @Description, Brand = @Brand, 
                Price = @Price, StockQuantity = @StockQuantity, Color = @Color, 
                Dimensions = @Dimensions, CategoryID = @CategoryID, IsUsed = @IsUsed, 
                MainImageUrl = @MainImageUrl, Discount = @Discount
            WHERE ProductID = @ProductId";

                using (var con = new SqlConnection(_connectionString))
                using (var updateCommand = new SqlCommand(updateString, con))
                {
                    updateCommand.Parameters.AddWithValue("@Name", product.Name);
                    updateCommand.Parameters.AddWithValue("@SKU", product.SKU);
                    updateCommand.Parameters.AddWithValue("@Description", product.Description);
                    updateCommand.Parameters.AddWithValue("@Brand", product.Brand);
                    updateCommand.Parameters.AddWithValue("@Price", product.Price);
                    updateCommand.Parameters.AddWithValue("@StockQuantity", product.StockQuantity);
                    updateCommand.Parameters.AddWithValue("@Color", product.Color);
                    updateCommand.Parameters.AddWithValue("@Dimensions", product.Dimensions);
                    updateCommand.Parameters.AddWithValue("@CategoryID", product.CategoryID);
                    updateCommand.Parameters.AddWithValue("@IsUsed", product.IsUsed);
                    updateCommand.Parameters.AddWithValue("@MainImageUrl", product.MainImageUrl);
                    updateCommand.Parameters.AddWithValue("@Discount", product.Discount ?? (object)DBNull.Value);
                    updateCommand.Parameters.AddWithValue("@ProductId", product.ProductID);

                    await con.OpenAsync();
                    await updateCommand.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating product: {ex.Message}");
                throw;
            }
        }


        public async Task DeleteProduct(int productId)
        {
            try
            {
                using (var con = new SqlConnection(_connectionString))
                {
                    await con.OpenAsync();

                    // Begin a transaction
                    using (var transaction = con.BeginTransaction())
                    {
                        try
                        {
                            // Delete from ProductImages table first
                            const string deleteImagesString = @"
                        DELETE FROM ProductImages
                        WHERE ProductID = @ProductId";

                            using (var deleteImagesCommand = new SqlCommand(deleteImagesString, con, transaction))
                            {
                                deleteImagesCommand.Parameters.AddWithValue("@ProductId", productId);
                                await deleteImagesCommand.ExecuteNonQueryAsync();
                            }

                            // Then delete from Products table
                            const string deleteProductString = @"
                        DELETE FROM Products
                        WHERE ProductID = @ProductId";

                            using (var deleteProductCommand = new SqlCommand(deleteProductString, con, transaction))
                            {
                                deleteProductCommand.Parameters.AddWithValue("@ProductId", productId);
                                await deleteProductCommand.ExecuteNonQueryAsync();
                            }

                            // Commit the transaction if all commands succeed
                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            // Rollback the transaction if any command fails
                            transaction.Rollback();
                            throw new Exception("Failed to delete product and associated images.", ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting product: {ex.Message}");
                throw;
            }
        }



        private Product GetProductFromReader(SqlDataReader productReader)
        {
            var product = new Product
            {
                ProductID = productReader.GetInt32(productReader.GetOrdinal("ProductID")),
                SKU = productReader.IsDBNull(productReader.GetOrdinal("SKU")) ? (short?)null : productReader.GetInt16(productReader.GetOrdinal("SKU")),
                Name = productReader.IsDBNull(productReader.GetOrdinal("Name")) ? null : productReader.GetString(productReader.GetOrdinal("Name")),
                Description = productReader.IsDBNull(productReader.GetOrdinal("Description")) ? null : productReader.GetString(productReader.GetOrdinal("Description")),
                Brand = productReader.IsDBNull(productReader.GetOrdinal("Brand")) ? null : productReader.GetString(productReader.GetOrdinal("Brand")),
                Price = productReader.IsDBNull(productReader.GetOrdinal("Price")) ? (decimal?)null : productReader.GetDecimal(productReader.GetOrdinal("Price")),
                StockQuantity = productReader.IsDBNull(productReader.GetOrdinal("StockQuantity")) ? (int?)null : productReader.GetInt32(productReader.GetOrdinal("StockQuantity")),
                Color = productReader.IsDBNull(productReader.GetOrdinal("Color")) ? null : productReader.GetString(productReader.GetOrdinal("Color")),
                Dimensions = productReader.IsDBNull(productReader.GetOrdinal("Dimensions")) ? null : productReader.GetString(productReader.GetOrdinal("Dimensions")),
                CategoryID = productReader.IsDBNull(productReader.GetOrdinal("CategoryID")) ? (int?)null : productReader.GetInt32(productReader.GetOrdinal("CategoryID")),
                IsUsed = productReader.IsDBNull(productReader.GetOrdinal("IsUsed")) ? (bool?)null : productReader.GetBoolean(productReader.GetOrdinal("IsUsed")),
                MainImageUrl = productReader.IsDBNull(productReader.GetOrdinal("MainImageUrl")) ? null : productReader.GetString(productReader.GetOrdinal("MainImageUrl")),
                Discount = productReader.IsDBNull(productReader.GetOrdinal("Discount")) ? (decimal?)null : productReader.GetDecimal(productReader.GetOrdinal("Discount")),
                Images = new List<ProductImage>()
            };

            // Add ProductImage if exists
            if (!productReader.IsDBNull(productReader.GetOrdinal("ImageID")))
            {
                var productImage = new ProductImage
                {
                    ImageID = productReader.GetInt32(productReader.GetOrdinal("ImageID")),
                    ProductID = product.ProductID,
                    ImageUrl = productReader.GetString(productReader.GetOrdinal("ImageUrl"))
                };

                product.Images.Add(productImage);
            }

            return product;
        }



        public async Task<List<Product>> GetProductsByParentCategoryId(int parentCategoryId)
        {
            var products = new List<Product>();

            try
            {
                const string queryString = @"
            SELECT p.ProductID, p.SKU, p.Name, p.Description, p.Brand, p.Price, p.StockQuantity, p.Color, p.Dimensions, p.CategoryID, p.IsUsed, p.MainImageUrl, p.Discount,
                   pi.ImageID, pi.ImageUrl
            FROM Products p
            INNER JOIN Categories c ON p.CategoryID = c.CategoryID
            INNER JOIN ParentCategories pc ON c.ParentCategoryID = pc.ParentCategoryID
            LEFT JOIN ProductImages pi ON p.ProductID = pi.ProductID
            WHERE pc.ParentCategoryID = @ParentCategoryID";

                using (var con = new SqlConnection(_connectionString))
                using (var command = new SqlCommand(queryString, con))
                {
                    command.Parameters.AddWithValue("@ParentCategoryID", parentCategoryId);

                    await con.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var productId = reader.GetInt32(reader.GetOrdinal("ProductID"));
                            var existingProduct = products.Find(p => p.ProductID == productId);

                            if (existingProduct == null)
                            {
                                var newProduct = GetProductFromReader(reader);
                                products.Add(newProduct);
                            }

                            // Add ProductImage if exists
                            if (!reader.IsDBNull(reader.GetOrdinal("ImageID")))
                            {
                                var productImage = new ProductImage
                                {
                                    ImageID = reader.GetInt32(reader.GetOrdinal("ImageID")),
                                    ProductID = productId,
                                    ImageUrl = reader.GetString(reader.GetOrdinal("ImageUrl"))
                                };

                                var productToAddImage = products.Find(p => p.ProductID == productId);
                                if (productToAddImage != null)
                                {
                                    productToAddImage.Images ??= new List<ProductImage>();
                                    productToAddImage.Images.Add(productImage);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving products by parent category ID: {ex.Message}");
                throw;
            }

            return products;
        }

        public async Task AddProductImages(int productId, List<string> imageUrls)
        {
            try
            {
                const string insertImageString = @"
                INSERT INTO ProductImages (ProductID, ImageUrl)
                VALUES (@ProductID, @ImageUrl)";

                using (var con = new SqlConnection(_connectionString))
                {
                    await con.OpenAsync();

                    foreach (var imageUrl in imageUrls)
                    {
                        using (var createImageCommand = new SqlCommand(insertImageString, con))
                        {
                            createImageCommand.Parameters.AddWithValue("@ProductID", productId);
                            createImageCommand.Parameters.AddWithValue("@ImageUrl", imageUrl);

                            await createImageCommand.ExecuteNonQueryAsync();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding product images: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteProductImages(int productId)
        {
            try
            {
                const string deleteImagesString = @"
                DELETE FROM ProductImages
                WHERE productID = @ProductId";

                using (var con = new SqlConnection(_connectionString))
                using (var deleteImagesCommand = new SqlCommand(deleteImagesString, con))
                {
                    deleteImagesCommand.Parameters.AddWithValue("@ProductId", productId);

                    await con.OpenAsync();
                    await deleteImagesCommand.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting product images: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteProductImage(int imageId)
        {
            try
            {
                const string deleteImageString = "DELETE FROM ProductImages WHERE imageID = @ImageId";

                using (var con = new SqlConnection(_connectionString))
                using (var deleteImageCommand = new SqlCommand(deleteImageString, con))
                {
                    deleteImageCommand.Parameters.AddWithValue("ImageId", imageId);

                    await con.OpenAsync();
                    await deleteImageCommand.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting product image: {ex.Message}");
                throw;
            }

        }

        public async Task<string> GetProductImageUrlById(int imageId)
        {
            string imageUrl = null;

            try
            {
                const string queryString = @"
            SELECT ImageUrl 
            FROM ProductImages 
            WHERE ImageID = @ImageId";

                using (var con = new SqlConnection(_connectionString))
                using (var command = new SqlCommand(queryString, con))
                {
                    command.Parameters.AddWithValue("@ImageId", imageId);

                    await con.OpenAsync();

                    imageUrl = (string)await command.ExecuteScalarAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving product image URL: {ex.Message}");
                throw;
            }

            return imageUrl;
        }



        public async Task<List<Product>> SearchProducts(string searchTerm)
        {
            var products = new List<Product>();

            try
            {
                const string queryString = @"
            SELECT p.ProductID, p.SKU, p.Name, p.Description, p.Brand, p.Price, p.StockQuantity, p.Color, p.Dimensions, p.CategoryID, p.IsUsed, p.MainImageUrl, p.Discount
            FROM Products p
            WHERE p.Name LIKE @SearchTerm OR p.Description LIKE @SearchTerm OR p.Brand LIKE @SearchTerm";

                using (var con = new SqlConnection(_connectionString))
                using (var command = new SqlCommand(queryString, con))
                {
                    command.Parameters.AddWithValue("@SearchTerm", $"%{searchTerm}%");

                    await con.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var product = new Product
                            {
                                ProductID = reader.GetInt32(reader.GetOrdinal("ProductID")),
                                SKU = reader.IsDBNull(reader.GetOrdinal("SKU")) ? (short?)null: reader.GetInt16(reader.GetOrdinal("SKU")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Description = reader.GetString(reader.GetOrdinal("Description")),
                                Brand = reader.GetString(reader.GetOrdinal("Brand")),
                                Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                                StockQuantity = reader.GetInt32(reader.GetOrdinal("StockQuantity")),
                                Color = reader.GetString(reader.GetOrdinal("Color")),
                                Dimensions = reader.GetString(reader.GetOrdinal("Dimensions")),
                                CategoryID = reader.GetInt32(reader.GetOrdinal("CategoryID")),
                                IsUsed = reader.GetBoolean(reader.GetOrdinal("IsUsed")) ? null : reader.GetBoolean(reader.GetOrdinal("IsUsed")),
                                MainImageUrl = reader.IsDBNull(reader.GetOrdinal("MainImageUrl")) ? null : reader.GetString(reader.GetOrdinal("MainImageUrl")),
                                Discount = reader.IsDBNull(reader.GetOrdinal("Discount")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("Discount")),
                            };
                            /*
                                     ProductID = productReader.GetInt32(productReader.GetOrdinal("ProductID")),
                SKU = productReader.IsDBNull(productReader.GetOrdinal("SKU")) ? (short?)null : productReader.GetInt16(productReader.GetOrdinal("SKU")),
                Name = productReader.IsDBNull(productReader.GetOrdinal("Name")) ? null : productReader.GetString(productReader.GetOrdinal("Name")),
                Description = productReader.IsDBNull(productReader.GetOrdinal("Description")) ? null : productReader.GetString(productReader.GetOrdinal("Description")),
                Brand = productReader.IsDBNull(productReader.GetOrdinal("Brand")) ? null : productReader.GetString(productReader.GetOrdinal("Brand")),
                Price = productReader.IsDBNull(productReader.GetOrdinal("Price")) ? (decimal?)null : productReader.GetDecimal(productReader.GetOrdinal("Price")),
                StockQuantity = productReader.IsDBNull(productReader.GetOrdinal("StockQuantity")) ? (int?)null : productReader.GetInt32(productReader.GetOrdinal("StockQuantity")),
                Color = productReader.IsDBNull(productReader.GetOrdinal("Color")) ? null : productReader.GetString(productReader.GetOrdinal("Color")),
                Dimensions = productReader.IsDBNull(productReader.GetOrdinal("Dimensions")) ? null : productReader.GetString(productReader.GetOrdinal("Dimensions")),
                CategoryID = productReader.IsDBNull(productReader.GetOrdinal("CategoryID")) ? (int?)null : productReader.GetInt32(productReader.GetOrdinal("CategoryID")),
                IsUsed = productReader.IsDBNull(productReader.GetOrdinal("IsUsed")) ? (bool?)null : productReader.GetBoolean(productReader.GetOrdinal("IsUsed")),
                MainImageUrl = productReader.IsDBNull(productReader.GetOrdinal("MainImageUrl")) ? null : productReader.GetString(productReader.GetOrdinal("MainImageUrl")),
                Discount = productReader.IsDBNull(productReader.GetOrdinal("Discount")) ? (decimal?)null : productReader.GetDecimal(productReader.GetOrdinal("Discount")),
                Images = new List<ProductImage>()
                             */
                            products.Add(product);
                        }
            
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error searching products: {ex.Message}");
                throw;
            }

            return products;
        }



    }


}
