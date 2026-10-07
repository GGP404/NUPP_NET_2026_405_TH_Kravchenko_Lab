using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShop.Common.Models
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

        // Static method
        public static Phone CreateNew()
        {
            int[] memories = { 64, 128, 256, 512 };

            return new Phone(
                $"Phone {Guid.NewGuid().ToString()[..8]}",
                Random.Shared.Next(10000, 80001),
                "Apple",
                memories[Random.Shared.Next(memories.Length)],
                "iOS");
        }

        // Method
        public override void ShowInfo()
        {
            Console.WriteLine(
                $"Телефон: {Brand} {Name}, {Memory} GB, {Price} грн");
        }
    }
}