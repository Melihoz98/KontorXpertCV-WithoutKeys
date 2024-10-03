using System.Collections.Generic;

namespace WebKontorExpert.Models
{
    public class Product
    {
        public Product()
        {
            Images = new List<ProductImage>();
        }

        public Product(string name, string description, string brand, decimal price, int stockQuantity, string color, string dimensions, int categoryID, bool isUsed, string mainImageUrl, short sku, decimal? discount = null)
            : this()
        {
            Name = name;
            Description = description;
            Brand = brand;
            Price = price;
            StockQuantity = stockQuantity;
            Color = color;
            Dimensions = dimensions;
            CategoryID = categoryID;
            IsUsed = isUsed;
            MainImageUrl = mainImageUrl;
            SKU = sku;
            Discount = discount;
        }

        public Product(int productID, string name, string description, string brand, decimal price, int stockQuantity, string color, string dimensions, int categoryID, bool isUsed, string mainImageUrl, short sku, decimal? discount = null)
            : this(name, description, brand, price, stockQuantity, color, dimensions, categoryID, isUsed, mainImageUrl, sku, discount)
        {
            ProductID = productID;
        }

        public int ProductID { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Brand { get; set; }
        public decimal? Price { get; set; }
        public int? StockQuantity { get; set; }
        public string? Color { get; set; }
        public string? Dimensions { get; set; }
        public int? CategoryID { get; set; }
        public bool? IsUsed { get; set; }
        public string? MainImageUrl { get; set; }
        public short? SKU { get; set; }
        public decimal? Discount { get; set; }  // Added Discount property

        // Navigation property to ProductImages
        public virtual ICollection<ProductImage> Images { get; set; }

        // Navigation properties (if any)
        // public virtual Category Category { get; set; }
        public virtual ParentCategory ParentCategory { get; set; }
        public int ParentCategoryID { get; set; }

        // Property to calculate PriceWithoutVAT
        public decimal? PriceWithoutVAT
        {
            get
            {
                if (Price.HasValue)
                {
                    return Price.Value / 1.25m;  // Remove VAT (25%)
                }
                return null;
            }
        }
        public decimal? DiscountPrice
        {
            get
            {
                if (Price.HasValue && Discount.HasValue && Discount.Value > 0)
                {
                    return Price.Value * (1 - Discount.Value / 100);
                }
                return 0;  // Return original price if no discount
            }
        }

        public decimal? DiscountPriceWithoutVAT
        {
            get
            {
                if (Price.HasValue && Discount.HasValue && Discount.Value > 0)
                {
                    // Calculate the discounted price
                    decimal discountedPrice = Price.Value * (1 - Discount.Value / 100);

                    // Remove VAT (25%)
                    return discountedPrice / 1.25m;
                }
                // Return the price without VAT if no discount is applied
                return 0;
            }
        }
    }
}
