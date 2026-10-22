namespace OnlineShop.Infrastructure.Models
{
    // Stores an order and its line items.
    public class OrderModel
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid CustomerId { get; set; }
        public CustomerModel Customer { get; set; } = null!;
        public ICollection<OrderItemModel> Items { get; set; } = new List<OrderItemModel>();
    }
}
