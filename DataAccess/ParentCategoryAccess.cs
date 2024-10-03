using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using WebKontorExpert.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace WebKontorExpert.DataAccess
{
    public class ParentCategoryAccess : IParentCategoryAccess
    {
        private readonly string _connectionString;

        public ParentCategoryAccess(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                                ?? throw new InvalidOperationException("Database connection string is not configured.");
        }

        public async Task<List<ParentCategory>> GetAllParentCategories()
        {
            var parentCategories = new List<ParentCategory>();

            try
            {
                const string queryString = "SELECT ParentCategoryID, ParentCategoryName FROM ParentCategories";

                using (var con = new SqlConnection(_connectionString))
                using (var readCommand = new SqlCommand(queryString, con))
                {
                    await con.OpenAsync();

                    using (var parentCategoryReader = await readCommand.ExecuteReaderAsync())
                    {
                        while (await parentCategoryReader.ReadAsync())
                        {
                            var parentCategory = GetParentCategoryFromReader(parentCategoryReader);
                            parentCategories.Add(parentCategory);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving parent categories: {ex.Message}");
                throw;
            }

            return parentCategories;
        }

        public async Task<ParentCategory> GetParentCategoryById(int parentCategoryId)
        {
            ParentCategory foundParentCategory = null;

            try
            {
                const string queryString = "SELECT ParentCategoryID, ParentCategoryName FROM ParentCategories WHERE ParentCategoryID = @ParentCategoryId";

                using (var con = new SqlConnection(_connectionString))
                using (var readCommand = new SqlCommand(queryString, con))
                {
                    readCommand.Parameters.AddWithValue("@ParentCategoryId", parentCategoryId);

                    await con.OpenAsync();

                    using (var parentCategoryReader = await readCommand.ExecuteReaderAsync())
                    {
                        if (await parentCategoryReader.ReadAsync())
                        {
                            foundParentCategory = GetParentCategoryFromReader(parentCategoryReader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving parent category: {ex.Message}");
                throw;
            }

            return foundParentCategory;
        }


        public async Task<ParentCategory> GetParentCategoryByName(string parentCategoryName)
        {
            ParentCategory parentCategory = null;
            const string queryString = "SELECT ParentCategoryID, ParentCategoryName FROM ParentCategories WHERE ParentCategoryName = @ParentCategoryName";

            using (var con = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(queryString, con))
            {
                command.Parameters.AddWithValue("@ParentCategoryName", parentCategoryName);

                await con.OpenAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        parentCategory = GetParentCategoryFromReader(reader);
                    }
                }
            }

            return parentCategory;
        }

        public async Task<int> AddParentCategory(ParentCategory parentCategory)
        {
            int insertedId = -1;

            try
            {
                const string insertString = "INSERT INTO ParentCategories (ParentCategoryName) OUTPUT INSERTED.ParentCategoryID VALUES (@ParentCategoryName)";

                using (var con = new SqlConnection(_connectionString))
                using (var createCommand = new SqlCommand(insertString, con))
                {
                    createCommand.Parameters.AddWithValue("@ParentCategoryName", parentCategory.ParentCategoryName);

                    await con.OpenAsync();
                    insertedId = (int)await createCommand.ExecuteScalarAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding parent category: {ex.Message}");
                throw;
            }

            return insertedId;
        }

        public async Task UpdateParentCategory(ParentCategory parentCategory)
        {
            try
            {
                const string updateString = "UPDATE ParentCategories SET ParentCategoryName = @ParentCategoryName WHERE ParentCategoryID = @ParentCategoryId";

                using (var con = new SqlConnection(_connectionString))
                using (var updateCommand = new SqlCommand(updateString, con))
                {
                    updateCommand.Parameters.AddWithValue("@ParentCategoryName", parentCategory.ParentCategoryName);
                    updateCommand.Parameters.AddWithValue("@ParentCategoryId", parentCategory.ParentCategoryID);

                    await con.OpenAsync();
                    await updateCommand.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating parent category: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteParentCategory(int parentCategoryId)
        {
            try
            {
                const string deleteString = "DELETE FROM ParentCategories WHERE ParentCategoryID = @ParentCategoryId";

                using (var con = new SqlConnection(_connectionString))
                using (var deleteCommand = new SqlCommand(deleteString, con))
                {
                    deleteCommand.Parameters.AddWithValue("@ParentCategoryId", parentCategoryId);

                    await con.OpenAsync();
                    await deleteCommand.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting parent category: {ex.Message}");
                throw;
            }
        }

        private ParentCategory GetParentCategoryFromReader(SqlDataReader parentCategoryReader)
        {
            int parentCategoryId = parentCategoryReader.GetInt32(parentCategoryReader.GetOrdinal("ParentCategoryID"));
            string parentCategoryName = parentCategoryReader.GetString(parentCategoryReader.GetOrdinal("ParentCategoryName"));

            return new ParentCategory(parentCategoryId, parentCategoryName);
        }
    }
}
