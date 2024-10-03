using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading.Tasks;
using WebKontorExpert.BusinessLogic;
using WebKontorExpert.Models;

namespace WebKontorExpert.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductData _productData;

        public HomeController(ILogger<HomeController> logger, IProductData productData)
        {
            _logger = logger;
            _productData = productData;
        }

        public async Task<IActionResult> Index()
        {
            // Fetch the latest products
            var latestProducts = await _productData.GetLatestProducts(8);

            // Pass the latest products to the view
            ViewBag.LatestProducts = latestProducts;

            return View();
        }


        public IActionResult WeBuy()
        {
            return View();
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
