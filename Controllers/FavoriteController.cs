using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebKontorExpert.Models;
using System.Linq;
using System.Threading.Tasks;
using WebKontorExpert.BusinessLogic;

namespace WebKontorExpert.Controllers
{
    public class FavoriteController : Controller
    {
        private const string FavoriteSessionKey = "Favorite";
        private readonly IProductData _productLogic;

        public FavoriteController(IProductData productLogic)
        {
            _productLogic = productLogic;
        }



  


        // Add a product to the favorites
        [HttpPost]
        public async Task<IActionResult> AddToFavorites(int productId)
        {
            try
            {
                var favorites = await GetFavoritesFromSessionAsync();
                var product = await _productLogic.GetProductById(productId);

                if (product != null)
                {
                    var favoriteItem = new FavoriteItem
                    {
                        ProductID = productId,
                        Name = product.Name,
                        Brand = product.Brand,
                        Price = product.Price,
                        MainImageUrl = product.MainImageUrl,
                        Discount = product.Discount
                    };

                    if (!favorites.FavoriteItems.Any(item => item.ProductID == productId))
                    {
                        favorites.FavoriteItems.Add(favoriteItem);
                        SaveFavoritesToSession(favorites);
                    }
                    return Json(new { success = true });
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                // Log the exception (e.g., using a logging framework or console)
                return StatusCode(500, new { error = ex.Message });
            }
        }


        // Remove a product from the favorites
        [HttpPost]
        public async Task<IActionResult> RemoveFromFavorites(int productId)
        {
            var favorites = await GetFavoritesFromSessionAsync();
            var itemToRemove = favorites.FavoriteItems.FirstOrDefault(item => item.ProductID == productId);

            if (itemToRemove != null)
            {
                favorites.FavoriteItems.Remove(itemToRemove);
                SaveFavoritesToSession(favorites);
                return Json(new { success = true });
            }
            return NotFound();
        }

        // Clear all favorites
        [HttpPost]
        public IActionResult ClearFavorites()
        {
            HttpContext.Session.Remove(FavoriteSessionKey);
            return Json(new { success = true });
        }

        // Get all favorite items
        [HttpGet]
        public async Task<IActionResult> GetFavorites()
        {
            var favorites = await GetFavoritesFromSessionAsync();
            return Json(new { count = favorites.FavoriteItems.Count, items = favorites.FavoriteItems });
        }


        private async Task<FavoriteList> GetFavoritesFromSessionAsync()
        {
            var favoritesJson = HttpContext.Session.GetString(FavoriteSessionKey);

            if (favoritesJson != null)
            {
                var favorites = JsonConvert.DeserializeObject<FavoriteList>(favoritesJson);
                return favorites;
            }

            return new FavoriteList();
        }

        private void SaveFavoritesToSession(FavoriteList favorites)
        {
            var favoritesJson = JsonConvert.SerializeObject(favorites);
            HttpContext.Session.SetString(FavoriteSessionKey, favoritesJson);
        }
    }
}