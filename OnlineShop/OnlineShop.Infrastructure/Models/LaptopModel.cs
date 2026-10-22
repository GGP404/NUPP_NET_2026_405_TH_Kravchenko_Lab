namespace OnlineShop.Infrastructure.Models
{
    // Stores the properties specific to laptop products.
    public class LaptopModel : ProductModel
    {
        public string Brand { get; set; } = string.Empty;
        public int Ram { get; set; }
        public string Processor { get; set; } = string.Empty;
    }
}
