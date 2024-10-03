using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Stripe;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebKontorExpert.BusinessLogic;
using WebKontorExpert.Models;

namespace WebKontorExpert.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IProductData _productData;

        public PaymentController(IConfiguration configuration, IProductData productData)
        {
            _configuration = configuration;
            _productData = productData;
        }

        [HttpGet]
        public async Task<IActionResult> Payment()
        {
            ViewBag.PublishableKey = _configuration["Stripe:PublishableKey"];

            var shoppingCart = await GetCartFromSessionAsync();

            var viewModel = new CheckoutViewModel
            {
                Customer = new WebKontorExpert.Models.Customer(), // Fully qualified name to avoid ambiguity
                CartItems = shoppingCart.CartItems.ToList() // Convert ICollection to List
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSession()
        {
            StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];

            var shoppingCart = await GetCartFromSessionAsync();

            var vatRate = 0.25m; // VAT rate of 25%

            var lineItems = shoppingCart.CartItems.Select(item =>
            {
                // Original price including VAT
                decimal priceInclVAT = item.Price ?? 0m;

                // Calculate discount
                decimal discountPercentage = item.Product.Discount ?? 0m;
                decimal discountAmount = priceInclVAT * (discountPercentage / 100);
                decimal discountPriceInclVAT = priceInclVAT - discountAmount;

                return new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(discountPriceInclVAT * 100), // Convert to cents
                        Currency = "dkk", // Use DKK for Danish Kroner
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = $"{item.Product.Brand} - {item.Product.Name}",
                            Images = IsValidUrl(item.Product.MainImageUrl) ? new List<string> { item.Product.MainImageUrl } : null,
                        },
                    },
                    Quantity = item.Quantity,
                };
            }).ToList();

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = lineItems,
                Mode = "payment",
                SuccessUrl = Url.Action("Success", "Payment", null, Request.Scheme),
                CancelUrl = Url.Action("Cancel", "Payment", null, Request.Scheme),
            };

            try
            {
                var service = new SessionService();
                Session session = await service.CreateAsync(options);

                return Ok(new { id = session.Id }); // Return a JSON response with the session ID
            }
            catch (StripeException e)
            {
                // Log the exception or handle it appropriately
                Console.WriteLine($"Stripe Exception: {e.Message}");
                return StatusCode(500, new { error = "Failed to create Stripe session" });
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                Console.WriteLine($"Unexpected Exception: {ex.Message}");
                return StatusCode(500, new { error = "Unexpected error occurred" });
            }
        }



        private bool IsValidUrl(string url)
        {
            return !string.IsNullOrEmpty(url) && Uri.TryCreate(url, UriKind.Absolute, out _);
        }

        private async Task<ShoppingCart> GetCartFromSessionAsync()
        {
            var cartJson = HttpContext.Session.GetString("Cart");

            if (cartJson != null)
            {
                var cart = JsonConvert.DeserializeObject<ShoppingCart>(cartJson);
                foreach (var item in cart.CartItems)
                {
                    if (item.Product == null || string.IsNullOrEmpty(item.Product.MainImageUrl))
                    {
                        // Retrieve product details if not already loaded or if MainImageUrl is empty
                        item.Product = await _productData.GetProductById(item.ProductID);
                        if (item.Product != null)
                        {
                            item.Price = item.Product.Price;
                            item.Quantity = item.Quantity;
                        }
                    }
                }
                return cart;
            }

            return new ShoppingCart();
        }

        public IActionResult Success()
        {
            return View();
        }

        public IActionResult Cancel()
        {
            return View();
        }
    }
}
