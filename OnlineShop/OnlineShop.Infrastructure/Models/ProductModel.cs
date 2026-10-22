namespace OnlineShop.Infrastructure.Models
{
    // Stores the shared properties of every product in the shop.
    public class ProductModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Price { get; set; }
        public ProductWarrantyModel? Warranty { get; set; }
    }
}
