namespace WebKontorExpert.Models
{
    public class FavoriteItem
    {
        public int ProductID { get; set; }
        public string Name { get; set; }
        public string Brand { get; set; }
        public decimal? Price { get; set; }

        public decimal? Discount { get; set; }
        public string MainImageUrl { get; set; }
    }
}