using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OnlineShop.Common;

namespace OnlineShop.ConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Online Shop ===");
            Console.WriteLine();

            // Creating a CRUD service
            CrudService<Product> service = new CrudService<Product>();

            // Creating a phone
            Phone phone = new Phone(
                "iPhone 15",
                30000,
                "Apple",
                128,
                "iOS");

            // Creating a laptop
            Laptop laptop = new Laptop(
                "Legion 5",
                50000,
                "Lenovo",
                16,
                "AMD Ryzen 7");

            // Creating a customer
            Customer customer = new Customer(
                "iван",
                "ivan@gmail.com",
                20);

            // Hooking up the event
            phone.PriceChanged += (oldPrice, newPrice) =>
            {
                Console.WriteLine(
                    $"Подiя: цiна змiнена з {oldPrice} на {newPrice}");
            };

            // Create
            Console.WriteLine("=== Create ===");

            service.Create(phone);
            service.Create(laptop);

            Console.WriteLine("Додано:");
            Console.WriteLine(phone.ShortInfo());
            Console.WriteLine(laptop.ShortInfo());

            Console.WriteLine();

            // Read all
            Console.WriteLine("=== Read all ===");

            foreach (Product product in service.ReadAll())
            {
                product.ShowInfo();
            }

            Console.WriteLine();

            // Read
            Console.WriteLine("=== Read ===");

            Product found = service.Read(phone.Id);

            Console.WriteLine("Знайдено:");
            found.ShowInfo();

            Console.WriteLine();

            // Update
            Console.WriteLine("=== Update ===");

            phone.ChangePrice(28000);
            service.Update(phone);

            Console.WriteLine("Новi данi:");
            phone.ShowInfo();

            Console.WriteLine();

            // Remove
            Console.WriteLine("=== Remove ===");

            service.Remove(laptop);

            Console.WriteLine("Ноутбук видалено.");

            Console.WriteLine();

            // Read all after remove
            Console.WriteLine("=== Read all after remove ===");

            foreach (Product product in service.ReadAll())
            {
                product.ShowInfo();
            }

            Console.WriteLine();

            // Customer
            Console.WriteLine("=== Customer ===");

            customer.ShowInfo();

            Console.WriteLine();

            // Static property
            Console.WriteLine(
                $"Кiлькiсть створених товарiв: {Product.Count}");

            // Static method
            Console.WriteLine(
                $"Новий ID: {Product.GenerateId()}");

            Console.ReadKey();
        }
    }
}