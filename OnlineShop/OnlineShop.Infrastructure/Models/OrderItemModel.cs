namespace OnlineShop.Infrastructure.Models
{
    // Stores a product line within an order.
    public class OrderItemModel
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public OrderModel Order { get; set; } = null!;
        public Guid ProductId { get; set; }
        public ProductModel Product { get; set; } = null!;
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
    }
}
