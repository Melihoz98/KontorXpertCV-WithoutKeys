namespace WebKontorExpert.Models
{
    public class ShoppingCart
    {
        public int ShoppingCartID { get; set; }
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}
