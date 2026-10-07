using System;
using System.Collections.Generic;
using System.Linq;
using OnlineShop.Common.Models;

namespace OnlineShop.ConsoleApp
{
    public static class Statistics
    {
        // Print statistics
        public static void Print(IEnumerable<Product> products)
        {
            // Materialize collection
            List<Product> productList = products.ToList();

            // Check if empty
            if (productList.Count == 0)
            {
                Console.WriteLine("Колекцiя порожня.");
                return;
            }

            // General statistics
            Console.WriteLine("=== LINQ статистика ===");

            Console.WriteLine(
                $"Цiна -> Min: {productList.Min(x => x.Price)}");

            Console.WriteLine(
                $"Цiна -> Max: {productList.Max(x => x.Price)}");

            Console.WriteLine(
                $"Цiна -> Average: {productList.Average(x => x.Price):F2}");

            // Filter phones
            List<Phone> phones = productList
                .OfType<Phone>()
                .ToList();

            // Phone memory statistics
            if (phones.Count > 0)
            {
                Console.WriteLine(
                    $"Пам'ять телефонiв -> Min: {phones.Min(x => x.Memory)} GB");

                Console.WriteLine(
                    $"Пам'ять телефонiв -> Max: {phones.Max(x => x.Memory)} GB");

                Console.WriteLine(
                    $"Пам'ять телефонiв -> Average: {phones.Average(x => x.Memory):F2} GB");
            }

            // Filter laptops
            List<Laptop> laptops = productList
                .OfType<Laptop>()
                .ToList();

            // Laptop RAM statistics
            if (laptops.Count > 0)
            {
                Console.WriteLine(
                    $"RAM ноутбукiв -> Min: {laptops.Min(x => x.Ram)} GB");

                Console.WriteLine(
                    $"RAM ноутбукiв -> Max: {laptops.Max(x => x.Ram)} GB");

                Console.WriteLine(
                    $"RAM ноутбукiв -> Average: {laptops.Average(x => x.Ram):F2} GB");
            }
        }
    }
}