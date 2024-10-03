namespace WebKontorExpert.Models
{
    public class ProductImage
    {
        public int ImageID { get; set; }
        public int ProductID { get; set; }
        public string ImageUrl { get; set; }

        // Navigation property til Product
        public virtual Product Product { get; set; }
    }
}
