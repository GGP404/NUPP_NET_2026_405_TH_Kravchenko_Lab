using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShop.Common.Models
{
    public class Customer
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int Age { get; set; }

        // Сonstructor
        public Customer(string name, string email, int age)
        {
            Id = Guid.NewGuid();
            Name = name;
            Email = email;
            Age = age;
        }

        // Static method
        public static Customer CreateNew()
        {
            return new Customer(
                $"Customer {Guid.NewGuid().ToString()[..8]}",
                $"user{Random.Shared.Next(1000, 9999)}@gmail.com",
                Random.Shared.Next(18, 71));
        }

        // Method
        public void ShowInfo()
        {
            Console.WriteLine($"{Name}, {Email}, {Age} рокiв");
        }
    }
}