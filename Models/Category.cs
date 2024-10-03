namespace WebKontorExpert.Models
{
    public class Category
    {
        //private int? parentCategoryId;

        public Category(int categoryId, string categoryName, int? parentCategoryId)
        {
            CategoryID = categoryId;
            CategoryName = categoryName;
            ParentCategoryID = parentCategoryId;
        }

        public Category() { }

        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public int? ParentCategoryID { get; set; }

        // Navigation properties
        public ParentCategory ParentCategory { get; set; }
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
