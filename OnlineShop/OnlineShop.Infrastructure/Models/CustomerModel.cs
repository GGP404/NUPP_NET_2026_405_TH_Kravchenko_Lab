namespace OnlineShop.Infrastructure.Models
{
    // Stores a customer and the orders created by that customer.
    public class CustomerModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int Age { get; set; }
        public ICollection<OrderModel> Orders { get; set; } = new List<OrderModel>();
    }
}
