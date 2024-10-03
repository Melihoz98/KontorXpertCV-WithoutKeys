using System.Collections.Generic;
using WebKontorExpert.Models;

namespace WebKontorExpert.Models
{
    public class ProductManagementViewModel
    {
        public List<ParentCategory> ParentCategories { get; set; } = new List<ParentCategory>();
        public List<Category> SubCategories { get; set; } = new List<Category>();
        public List<Product> Products { get; set; } = new List<Product>();
        public int? SelectedParentCategoryID { get; set; }
        public int? SelectedSubCategoryID { get; set; }
    }
}

