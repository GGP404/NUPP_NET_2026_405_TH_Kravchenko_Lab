using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OnlineShop.Common.Models;
using OnlineShop.Common.Services;
using Xunit;

namespace OnlineShop.Tests
{
    public class CrudServiceAsyncTests
    {
        private static string CreateTempFile()
        {
            return Path.Combine(
                Path.GetTempPath(),
                $"products-{Guid.NewGuid()}.json");
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateProduct()
        {
            string filePath = CreateTempFile();

            try
            {
                CrudServiceAsync<Product> service =
                    new CrudServiceAsync<Product>(filePath);

                Phone phone = Phone.CreateNew();

                bool result =
                    await service.CreateAsync(phone);

                Assert.True(result);

                var items =
                    await service.ReadAllAsync();

                Assert.Single(items);
                Assert.Equal(phone.Id, items.First().Id);
            }
            finally
            {
                DeleteFile(filePath);
            }
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnFalseForDuplicate()
        {
            string filePath = CreateTempFile();

            try
            {
                CrudServiceAsync<Product> service =
                    new CrudServiceAsync<Product>(filePath);

                Phone phone = Phone.CreateNew();

                Assert.True(
                    await service.CreateAsync(phone));

                Assert.False(
                    await service.CreateAsync(phone));
            }
            finally
            {
                DeleteFile(filePath);
            }
        }

        [Fact]
        public async Task ReadAsync_ShouldReturnProduct()
        {
            string filePath = CreateTempFile();

            try
            {
                CrudServiceAsync<Product> service =
                    new CrudServiceAsync<Product>(filePath);

                Phone phone = Phone.CreateNew();

                await service.CreateAsync(phone);

                Product result =
                    await service.ReadAsync(phone.Id);

                Assert.NotNull(result);
                Assert.Equal(phone.Id, result.Id);
            }
            finally
            {
                DeleteFile(filePath);
            }
        }

        [Fact]
        public async Task ReadAsync_ShouldReturnNullForMissingId()
        {
            string filePath = CreateTempFile();

            try
            {
                CrudServiceAsync<Product> service =
                    new CrudServiceAsync<Product>(filePath);

                Product? result =
                    await service.ReadAsync(Guid.NewGuid());

                Assert.Null(result);
            }
            finally
            {
                DeleteFile(filePath);
            }
        }

        [Fact]
        public async Task ReadAllAsync_WithPagination_ShouldReturnCorrectPage()
        {
            string filePath = CreateTempFile();

            try
            {
                CrudServiceAsync<Product> service =
                    new CrudServiceAsync<Product>(filePath);

                for (int i = 0; i < 10; i++)
                {
                    await service.CreateAsync(
                        Phone.CreateNew());
                }

                var page =
                    await service.ReadAllAsync(2, 3);

                Assert.Equal(3, page.Count());
            }
            finally
            {
                DeleteFile(filePath);
            }
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateExistingProduct()
        {
            string filePath = CreateTempFile();

            try
            {
                CrudServiceAsync<Product> service =
                    new CrudServiceAsync<Product>(filePath);

                Phone phone = Phone.CreateNew();

                await service.CreateAsync(phone);

                phone.Price = 99999;

                bool result =
                    await service.UpdateAsync(phone);

                Assert.True(result);

                Product updated =
                    await service.ReadAsync(phone.Id);

                Assert.Equal(99999, updated.Price);
            }
            finally
            {
                DeleteFile(filePath);
            }
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnFalseForMissingProduct()
        {
            string filePath = CreateTempFile();

            try
            {
                CrudServiceAsync<Product> service =
                    new CrudServiceAsync<Product>(filePath);

                Phone phone = Phone.CreateNew();

                bool result =
                    await service.UpdateAsync(phone);

                Assert.False(result);
            }
            finally
            {
                DeleteFile(filePath);
            }
        }

        [Fact]
        public async Task RemoveAsync_ShouldRemoveProduct()
        {
            string filePath = CreateTempFile();

            try
            {
                CrudServiceAsync<Product> service =
                    new CrudServiceAsync<Product>(filePath);

                Phone phone = Phone.CreateNew();

                await service.CreateAsync(phone);

                bool result =
                    await service.RemoveAsync(phone);

                Assert.True(result);

                var items =
                    await service.ReadAllAsync();

                Assert.Empty(items);
            }
            finally
            {
                DeleteFile(filePath);
            }
        }

        [Fact]
        public async Task RemoveAsync_ShouldReturnFalseForMissingProduct()
        {
            string filePath = CreateTempFile();

            try
            {
                CrudServiceAsync<Product> service =
                    new CrudServiceAsync<Product>(filePath);

                Phone phone = Phone.CreateNew();

                bool result =
                    await service.RemoveAsync(phone);

                Assert.False(result);
            }
            finally
            {
                DeleteFile(filePath);
            }
        }

        [Fact]
        public async Task SaveAsync_ShouldCreateJsonFile()
        {
            string filePath = CreateTempFile();

            try
            {
                CrudServiceAsync<Product> service =
                    new CrudServiceAsync<Product>(filePath);

                Phone phone = Phone.CreateNew();

                await service.CreateAsync(phone);

                bool result =
                    await service.SaveAsync();

                Assert.True(result);
                Assert.True(File.Exists(filePath));

                string json =
                    await File.ReadAllTextAsync(filePath);

                Assert.Contains(phone.Name, json);
            }
            finally
            {
                DeleteFile(filePath);
            }
        }

        [Fact]
        public async Task Service_ShouldBeThreadSafe()
        {
            string filePath = CreateTempFile();

            try
            {
                CrudServiceAsync<Product> service =
                    new CrudServiceAsync<Product>(filePath);

                Task<bool>[] tasks =
                    Enumerable
                        .Range(0, 1000)
                        .Select(_ =>
                            service.CreateAsync(
                                Phone.CreateNew()))
                        .ToArray();

                bool[] results =
                    await Task.WhenAll(tasks);

                Assert.All(
                    results,
                    result => Assert.True(result));

                var items =
                    await service.ReadAllAsync();

                Assert.Equal(1000, items.Count());
            }
            finally
            {
                DeleteFile(filePath);
            }
        }

        [Fact]
        public async Task Service_ShouldSupportIEnumerable()
        {
            string filePath = CreateTempFile();

            try
            {
                CrudServiceAsync<Product> service =
                    new CrudServiceAsync<Product>(filePath);

                await service.CreateAsync(
                    Phone.CreateNew());

                await service.CreateAsync(
                    Laptop.CreateNew());

                int count = service.Count();

                Assert.Equal(2, count);
            }
            finally
            {
                DeleteFile(filePath);
            }
        }

        private static void DeleteFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}