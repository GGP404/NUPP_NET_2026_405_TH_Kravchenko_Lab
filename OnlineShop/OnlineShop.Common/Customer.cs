using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShop.Common
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

        // Method
        public void ShowInfo()
        {
            Console.WriteLine($"{Name}, {Email}, {Age} рокiв");
        }
    }
}