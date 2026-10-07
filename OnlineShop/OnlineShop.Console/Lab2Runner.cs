using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OnlineShop.Common.Models;
using OnlineShop.Common.Services;

namespace OnlineShop.ConsoleApp
{
    public static class Lab2Runner
    {
        public static async Task RunAsync()
        {
            // Header
            Console.WriteLine("=== Лабораторна робота №2 ===");
            Console.WriteLine();

            // Path to JSON file
            string filePath = Path.Combine(
                AppContext.BaseDirectory,
                "products-lab2.json");

            // Creating an asynchronous CRUD service
            CrudServiceAsync<Product> service =
                new CrudServiceAsync<Product>(filePath);

            const int amount = 1000;

            // Parallel creation of products
            Console.WriteLine(
                $"Створення {amount} об'єктів у паралельному режимі...");

            var createdProducts =
                await Parallell.CreateProductsAsync(
                    service,
                    amount);

            Console.WriteLine(
                $"Створено: {createdProducts.Count} об'єктів");

            Console.WriteLine();

            // IEnumerable execution
            Console.WriteLine("=== IEnumerable ===");

            foreach (Product product in service.Take(5))
            {
                Console.WriteLine(
                    $"{product.Name} - {product.Price} грн");
            }

            Console.WriteLine();

            // Read all asynchronously
            Console.WriteLine("=== ReadAllAsync ===");

            var allProducts =
                await service.ReadAllAsync();

            Console.WriteLine(
                $"Кількість об'єктів: {allProducts.Count()}");

            Console.WriteLine();

            // Pagination
            Console.WriteLine("=== Pagination ===");

            var firstPage =
                await service.ReadAllAsync(1, 10);

            Console.WriteLine(
                $"Перша сторінка містить: {firstPage.Count()} об'єктів");

            Console.WriteLine();

            // Statistics
            Statistics.Print(allProducts);

            Console.WriteLine();

            // Save asynchronously
            Console.WriteLine("=== SaveAsync ===");

            bool saved = await service.SaveAsync();

            Console.WriteLine(
                saved
                    ? $"Колекцію збережено у файл: {service.FilePath}"
                    : "Помилка збереження.");

            Console.WriteLine();

            // Synchronization primitives
            Console.WriteLine("=== Synchronization primitives ===");

            // Lock example
            SynchronizationExamples.LockExample();

            // Semaphore example
            await SynchronizationExamples.SemaphoreExampleAsync();

            // AutoResetEvent example
            SynchronizationExamples.AutoResetEventExample();

            Console.WriteLine();

            // Finish
            Console.WriteLine("=== Laboratory work №2 finished ===");
        }
    }
}