namespace OnlineShop.Infrastructure.Models
{
    // Stores the properties specific to phone products.
    public class PhoneModel : ProductModel
    {
        public string Brand { get; set; } = string.Empty;
        public int Memory { get; set; }
        public string OperatingSystem { get; set; } = string.Empty;
    }
}
