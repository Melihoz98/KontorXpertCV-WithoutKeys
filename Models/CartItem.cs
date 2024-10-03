using WebKontorExpert.Models;

namespace WebKontorExpert.Models
{
    public class CartItem
    {
        public int CartItemId { get; set; }
        public int ShoppingCartID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public decimal ?Price { get; set; } // Optional: If you want to store the price at the time of adding to cart

        // Navigation property
        public Product Product { get; set; }
        public ShoppingCart ShoppingCart { get; set; }
    }
}
