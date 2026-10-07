using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OnlineShop.Common.Models;

namespace OnlineShop.Common.Services
{
    public static class Parallell
    {
        // Parallel creation of products
        public static async Task<IReadOnlyList<Product>> CreateProductsAsync(
            CrudServiceAsync<Product> service,
            int amount)
        {
            // Validation
            if (amount < 1)
                throw new ArgumentOutOfRangeException(nameof(amount));

            // Thread-safe collection
            ConcurrentBag<Product> products = new();

            // Parallel generation of products
            Parallel.For(
                0,
                amount,
                i =>
                {
                    Product product;

                    // Alternating product types
                    if (i % 2 == 0)
                    {
                        product = Phone.CreateNew();
                    }
                    else
                    {
                        product = Laptop.CreateNew();
                    }

                    // Thread-safe addition
                    products.Add(product);
                });

            // Asynchronous creation in CRUD service
            await Task.WhenAll(
                products.Select(service.CreateAsync));

            // Return result
            return products.ToList();
        }
    }
}