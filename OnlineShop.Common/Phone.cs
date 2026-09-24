using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShop.Common
{
    public class Phone : Product
    {
        public string Brand { get; set; }
        public int Memory { get; set; }
        public string OperatingSystem { get; set; }

        // Constructor
        public Phone(
            string name,
            double price,
            string brand,
            int memory,
            string operatingSystem)
            : base(name, price)
        {
            Brand = brand;
            Memory = memory;
            OperatingSystem = operatingSystem;
        }

        // Method
        public override void ShowInfo()
        {
            Console.WriteLine(
                $"Телефон: {Brand} {Name}, {Memory} GB, {Price} грн");
        }
    }
}