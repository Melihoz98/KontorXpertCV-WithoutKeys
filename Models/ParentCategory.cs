namespace WebKontorExpert.Models
{
    public class ParentCategory
    {
        public ParentCategory(int parentCategoryId, string parentCategoryName)
        {
            ParentCategoryID = parentCategoryId;
            ParentCategoryName = parentCategoryName;
        }

        public ParentCategory() { }

        public int ParentCategoryID { get; set; }
        public string ParentCategoryName { get; set; }

        // Navigation properties
        public ICollection<Category> Categories { get; set; } = new List<Category>();

        // Navigation property to access products indirectly
        public ICollection<Product> Products { get; set; }
    }
}
