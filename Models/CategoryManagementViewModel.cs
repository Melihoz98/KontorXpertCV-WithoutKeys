namespace WebKontorExpert.Models
{
    public class CategoryManagementViewModel
    {
        public List<ParentCategory> ParentCategories { get; set; }
        public List<Category> Categories { get; set; }
        public Category NewCategory { get; set; }
        public ParentCategory NewParentCategory { get; set; }
    }
}
