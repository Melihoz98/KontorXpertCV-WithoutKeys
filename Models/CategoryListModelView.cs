namespace WebKontorExpert.Models
{
    public class CategoryListModelView
    {
        public ICollection<ParentCategory> ParentCategories { get; set; }
        public ICollection<Category> Categories { get; set; }
    }
}
