using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShop.Common
{
    public class Laptop : Product
    {
        public string Brand { get; set; }
        public int Ram { get; set; }
        public string Processor { get; set; }

        // Constructor
        public Laptop(
            string name,
            double price,
            string brand,
            int ram,
            string processor)
            : base(name, price)
        {
            Brand = brand;
            Ram = ram;
            Processor = processor;
        }

        // Method
        public override void ShowInfo()
        {
            Console.WriteLine(
                $"Ноутбук: {Brand} {Name}, RAM: {Ram} GB, {Price} грн");
        }
    }
}