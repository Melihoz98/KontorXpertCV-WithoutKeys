using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebKontorExpert.Models;
using System.Linq;
using System.Threading.Tasks;
using WebKontorExpert.BusinessLogic;

namespace WebKontorExpert.Controllers
{
    public class ShoppingCartController : Controller
    {
        private const string CartSessionKey = "Cart";
        private readonly IProductData _productLogic;

        public ShoppingCartController(IProductData productLogic)
        {
            _productLogic = productLogic;
        }

        public async Task<IActionResult> Index()
        {
            var cart = await GetCartFromSessionAsync();
            return View(cart);
        }

        public async Task<IActionResult> AddToCart(int productId, int quantity)
        {
            var cart = await GetCartFromSessionAsync();

            var existingItem = cart.CartItems.FirstOrDefault(item => item.ProductID == productId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                var product = await _productLogic.GetProductById(productId);
                if (product != null)
                {
                    cart.CartItems.Add(new CartItem { ProductID = productId, Quantity = quantity, Product = product, Price = product.Price });
                }
                else
                {
                    return NotFound();
                }
            }

            SaveCartToSession(cart);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AddToCartBtn(int productId, int quantity)
        {
            var cart = await GetCartFromSessionAsync();
            var existingItem = cart.CartItems.FirstOrDefault(item => item.ProductID == productId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                var product = await _productLogic.GetProductById(productId);
                if (product != null)
                {
                    cart.CartItems.Add(new CartItem { ProductID = productId, Quantity = quantity, Product = product, Price = product.Price });
                }
                else
                {
                    return Json(new { success = false, error = "Product not found" });
                }
            }

            SaveCartToSession(cart);

            return Json(new { success = true });
        }


        public async Task<IActionResult> RemoveFromCart(int productId)
        {
            var cart = await GetCartFromSessionAsync();

            var itemToRemove = cart.CartItems.FirstOrDefault(item => item.ProductID == productId);

            if (itemToRemove != null)
            {
                cart.CartItems.Remove(itemToRemove);
                SaveCartToSession(cart);
            }

            return RedirectToAction("Index");
        }

        public IActionResult ClearCart()
        {
            HttpContext.Session.Remove(CartSessionKey);
            return RedirectToAction("Index");
        }

        private async Task<ShoppingCart> GetCartFromSessionAsync()
        {
            var cartJson = HttpContext.Session.GetString(CartSessionKey);

            if (cartJson != null)
            {
                var cart = JsonConvert.DeserializeObject<ShoppingCart>(cartJson);
                foreach (var item in cart.CartItems)
                {
                    if (item.Product == null)
                    {
                        // Use await to asynchronously get the product
                        item.Product = await _productLogic.GetProductById(item.ProductID);
                        if (item.Product != null)
                        {
                            item.Price = item.Product.Price; // Ensure price is also set correctly
                        }
                    }
                }
                return cart;
            }

            return new ShoppingCart();
        }

        private void SaveCartToSession(ShoppingCart cart)
        {
            var cartJson = JsonConvert.SerializeObject(cart);
            HttpContext.Session.SetString(CartSessionKey, cartJson);
        }
        [HttpGet]
        public async Task<IActionResult> GetCartItemCount()
        {
            var cart = await GetCartFromSessionAsync();
            int cartItemCount = cart.CartItems.Sum(item => item.Quantity);
            return Json(new { count = cartItemCount });
        }

    }
}
