using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShop.Common
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }

        // Static field
        private static int _count;

        public static int Count
        {
            get { return _count; }
        }

        // Delegate
        public delegate void PriceChangedHandler(double oldPrice, double newPrice);

        // Event
        public event PriceChangedHandler PriceChanged;

        // Static constructor
        static Product()
        {
            _count = 0;
        }

        // Default constructor
        public Product(string name, double price)
        {
            Id = Guid.NewGuid();
            Name = name;
            Price = price;

            _count++;
        }

        // Method
        public void ChangePrice(double newPrice)
        {
            double oldPrice = Price;
            Price = newPrice;

            PriceChanged?.Invoke(oldPrice, newPrice);
        }

        // Static method
        public static Guid GenerateId()
        {
            return Guid.NewGuid();
        }

        // Default method
        public virtual void ShowInfo()
        {
            Console.WriteLine($"{Name} - {Price} грн");
        }
    }
}