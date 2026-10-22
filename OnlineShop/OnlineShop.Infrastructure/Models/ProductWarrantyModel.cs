namespace OnlineShop.Infrastructure.Models
{
    // Stores the one-to-one warranty record associated with a product.
    public class ProductWarrantyModel
    {
        public Guid ProductId { get; set; }
        public int PeriodMonths { get; set; }
        public string Provider { get; set; } = string.Empty;
        public ProductModel Product { get; set; } = null!;
    }
}
