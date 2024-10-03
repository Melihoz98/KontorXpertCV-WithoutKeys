using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using WebKontorExpert.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace WebKontorExpert.DataAccess
{
    public class CategoryAccess : ICategoryAccess
    {
        private readonly string _connectionString;

        public CategoryAccess(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                                ?? throw new InvalidOperationException("Database connection string is not configured.");
        }

        public async Task<List<Category>> GetAllCategories()
        {
            var categories = new List<Category>();

            try
            {
                const string queryString = "SELECT CategoryID, CategoryName, ParentCategoryID FROM Categories";

                using (var con = new SqlConnection(_connectionString))
                using (var readCommand = new SqlCommand(queryString, con))
                {
                    await con.OpenAsync();

                    using (var categoryReader = await readCommand.ExecuteReaderAsync())
                    {
                        while (await categoryReader.ReadAsync())
                        {
                            var category = GetCategoryFromReader(categoryReader);
                            categories.Add(category);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving categories: {ex.Message}");
                throw;
            }

            return categories;
        }

        public async Task<Category> GetCategoryById(int categoryId)
        {
            Category foundCategory = null;

            try
            {
                const string queryString = "SELECT CategoryID, CategoryName, ParentCategoryID FROM Categories WHERE CategoryID = @CategoryId";

                using (var con = new SqlConnection(_connectionString))
                using (var readCommand = new SqlCommand(queryString, con))
                {
                    readCommand.Parameters.AddWithValue("@CategoryId", categoryId);

                    await con.OpenAsync();

                    using (var categoryReader = await readCommand.ExecuteReaderAsync())
                    {
                        if (await categoryReader.ReadAsync())
                        {
                            foundCategory = GetCategoryFromReader(categoryReader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving category: {ex.Message}");
                throw;
            }

            return foundCategory;
        }

        public async Task<Category> GetCategoryByName(string categoryName)
        {
            Category category = null;
            const string queryString = "SELECT CategoryID, CategoryName, ParentCategoryID FROM Categories WHERE CategoryName = @CategoryName";

            using (var con = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(queryString, con))
            {
                command.Parameters.AddWithValue("@CategoryName", categoryName);

                await con.OpenAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        category = GetCategoryFromReader(reader);
                    }
                }
            }

            return category;
        }

        public async Task<List<Category>> GetCategoriesByParentCategoryId(int parentCategoryId)
        {
            var categories = new List<Category>();

            try
            {
                const string queryString = "SELECT CategoryID, CategoryName, ParentCategoryID FROM Categories WHERE ParentCategoryID = @ParentCategoryId";

                using (var con = new SqlConnection(_connectionString))
                using (var readCommand = new SqlCommand(queryString, con))
                {
                    readCommand.Parameters.AddWithValue("@ParentCategoryId", parentCategoryId);

                    await con.OpenAsync();

                    using (var categoryReader = await readCommand.ExecuteReaderAsync())
                    {
                        while (await categoryReader.ReadAsync())
                        {
                            var category = GetCategoryFromReader(categoryReader);
                            categories.Add(category);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving categories for parent category: {ex.Message}");
                throw;
            }

            return categories;
        }

        public async Task<int> AddCategory(Category category)
        {
            int insertedId = -1;

            try
            {
                const string insertString = "INSERT INTO Categories (CategoryName, ParentCategoryID) OUTPUT INSERTED.CategoryID VALUES (@CategoryName, @ParentCategoryID)";

                using (var con = new SqlConnection(_connectionString))
                using (var createCommand = new SqlCommand(insertString, con))
                {
                    createCommand.Parameters.AddWithValue("@CategoryName", category.CategoryName);
                    createCommand.Parameters.AddWithValue("@ParentCategoryID", (object)category.ParentCategoryID ?? DBNull.Value);

                    await con.OpenAsync();
                    insertedId = (int)await createCommand.ExecuteScalarAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding category: {ex.Message}");
                throw;
            }

            return insertedId;
        }

        public async Task UpdateCategory(Category category)
        {
            try
            {
                const string updateString = "UPDATE Categories SET CategoryName = @CategoryName, ParentCategoryID = @ParentCategoryID WHERE CategoryID = @CategoryId";

                using (var con = new SqlConnection(_connectionString))
                using (var updateCommand = new SqlCommand(updateString, con))
                {
                    updateCommand.Parameters.AddWithValue("@CategoryName", category.CategoryName);
                    updateCommand.Parameters.AddWithValue("@CategoryId", category.CategoryID);
                    updateCommand.Parameters.AddWithValue("@ParentCategoryID", (object)category.ParentCategoryID ?? DBNull.Value);

                    await con.OpenAsync();
                    await updateCommand.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating category: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteCategory(int categoryId)
        {
            try
            {
                const string deleteString = "DELETE FROM Categories WHERE CategoryID = @CategoryId";

                using (var con = new SqlConnection(_connectionString))
                using (var deleteCommand = new SqlCommand(deleteString, con))
                {
                    deleteCommand.Parameters.AddWithValue("@CategoryId", categoryId);

                    await con.OpenAsync();
                    await deleteCommand.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting category: {ex.Message}");
                throw;
            }
        }

        private Category GetCategoryFromReader(SqlDataReader categoryReader)
        {
            int categoryId = categoryReader.GetInt32(categoryReader.GetOrdinal("CategoryID"));
            string categoryName = categoryReader.GetString(categoryReader.GetOrdinal("CategoryName"));
            int? parentCategoryId = categoryReader.IsDBNull(categoryReader.GetOrdinal("ParentCategoryID")) ? (int?)null : categoryReader.GetInt32(categoryReader.GetOrdinal("ParentCategoryID"));

            return new Category(categoryId, categoryName, parentCategoryId);
        }
    }
}
