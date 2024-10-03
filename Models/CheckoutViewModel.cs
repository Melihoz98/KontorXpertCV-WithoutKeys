namespace WebKontorExpert.Models
{
    public class CheckoutViewModel
    {
        public Customer Customer { get; set; }
        public ShoppingCart ShoppingCart { get; set; } = new ShoppingCart();

        public List<CartItem> CartItems { get; set; } = new List<CartItem>();

    }
}