using Microsoft.EntityFrameworkCore;
using OnlineShop.Common.Services;
using OnlineShop.Common.Repositories;
using OnlineShop.Infrastructure;
using OnlineShop.Infrastructure.Models;
using OnlineShop.Infrastructure.Repositories;

namespace OnlineShop.ConsoleApp
{
    // Runs the database-backed CRUD workflow for the third laboratory assignment.
    public static class Lab3Runner
    {
        private static readonly Guid PhoneId = Guid.Parse("a1100000-0000-4000-8000-000000000001");
        private static readonly Guid LaptopId = Guid.Parse("a1100000-0000-4000-8000-000000000002");
        private static readonly Guid CustomerId = Guid.Parse("c1100000-0000-4000-8000-000000000001");

        public static async Task RunAsync()
        {
            Console.WriteLine("=== Laboratory work №3: Entity Framework Core ===");

            await using var context = new OnlineShopContext();
            await context.Database.MigrateAsync();

            IRepository<ProductModel> repository = new EntityFrameworkRepository<ProductModel>(context);
            var service = new CrudServiceAsync<ProductModel>(repository);

            // Creates stable sample records when the database does not contain them yet.
            ProductModel? existingPhone = await service.ReadAsync(PhoneId);
            PhoneModel phone;
            if (existingPhone is PhoneModel storedPhone)
            {
                phone = storedPhone;
            }
            else
            {
                phone = new PhoneModel
                {
                    Id = PhoneId,
                    Name = "iPhone 15",
                    Price = 30000,
                    Brand = "Apple",
                    Memory = 128,
                    OperatingSystem = "iOS",
                    Warranty = new ProductWarrantyModel
                    {
                        ProductId = PhoneId,
                        PeriodMonths = 24,
                        Provider = "OnlineShop"
                    }
                };
                await service.CreateAsync(phone);
            }

            ProductModel? existingLaptop = await service.ReadAsync(LaptopId);
            if (existingLaptop is not LaptopModel)
            {
                await service.CreateAsync(new LaptopModel
                {
                    Id = LaptopId,
                    Name = "Legion 5",
                    Price = 50000,
                    Brand = "Lenovo",
                    Ram = 16,
                    Processor = "AMD Ryzen 7"
                });
            }

            // Ensures that the one-to-one warranty relationship is present for the sample phone.
            if (!await context.ProductWarranties.AnyAsync(warranty => warranty.ProductId == PhoneId))
            {
                context.ProductWarranties.Add(new ProductWarrantyModel
                {
                    ProductId = PhoneId,
                    PeriodMonths = 24,
                    Provider = "OnlineShop"
                });
                await context.SaveChangesAsync();
            }

            // Creates related customer orders to demonstrate the one-to-many relationships.
            CustomerModel? customer = await context.Customers.FindAsync(CustomerId);
            if (customer is null)
            {
                customer = new CustomerModel
                {
                    Id = CustomerId,
                    Name = "Alex Morgan",
                    Email = "alex.morgan@example.com",
                    Age = 25
                };
                context.Customers.Add(customer);
            }

            if (!await context.Orders.AnyAsync(order => order.CustomerId == CustomerId))
            {
                customer.Orders.Add(new OrderModel
                {
                    Id = Guid.Parse("c2100000-0000-4000-8000-000000000001"),
                    CustomerId = CustomerId,
                    CreatedAt = DateTime.UtcNow,
                    Items =
                    {
                        new OrderItemModel
                        {
                            Id = Guid.Parse("c3100000-0000-4000-8000-000000000001"),
                            ProductId = PhoneId,
                            Quantity = 1,
                            UnitPrice = phone.Price
                        }
                    }
                });
                customer.Orders.Add(new OrderModel
                {
                    Id = Guid.Parse("c2100000-0000-4000-8000-000000000002"),
                    CustomerId = CustomerId,
                    CreatedAt = DateTime.UtcNow,
                    Items =
                    {
                        new OrderItemModel
                        {
                            Id = Guid.Parse("c3100000-0000-4000-8000-000000000002"),
                            ProductId = LaptopId,
                            Quantity = 1,
                            UnitPrice = 50000
                        }
                    }
                });
                await context.SaveChangesAsync();
            }

            // Reads and displays persisted products.
            Console.WriteLine("--- Read all products ---");
            var products = await service.ReadAllAsync();
            foreach (ProductModel product in products)
            {
                Console.WriteLine($"{product.Name} | {product.Price:F2} UAH | {product.GetType().Name}");
            }
            Console.WriteLine($"Products in database: {products.Count()}");

            // Demonstrates paginated repository reads.
            var firstPage = await service.ReadAllAsync(1, 2);
            Console.WriteLine($"First page contains {firstPage.Count()} product(s).");

            // Updates an existing product through the repository-backed CRUD service.
            phone.Price = 27999;
            bool updated = await service.UpdateAsync(phone);
            Console.WriteLine(updated
                ? $"Updated {phone.Name}: {phone.Price:F2} UAH"
                : "The sample phone was not found for update.");

            // Creates and removes a temporary row to demonstrate the complete CRUD cycle.
            var temporaryProduct = new LaptopModel
            {
                Id = Guid.NewGuid(),
                Name = "Temporary product",
                Price = 1,
                Brand = "Demo",
                Ram = 8,
                Processor = "Demo CPU"
            };
            bool created = await service.CreateAsync(temporaryProduct);
            ProductModel? readTemporary = await service.ReadAsync(temporaryProduct.Id);
            bool removed = readTemporary is not null && await service.RemoveAsync(readTemporary);
            Console.WriteLine($"Temporary product: created={created}, read={readTemporary is not null}, removed={removed}");

            await service.SaveAsync();
            Console.WriteLine($"SQLite database: {context.Database.GetDbConnection().DataSource}");
            Console.WriteLine("=== Laboratory work №3 finished ===");
            Console.WriteLine();
        }
    }
}
