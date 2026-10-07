using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using OnlineShop.Common.Models;

namespace OnlineShop.Common.Services
{
    public class CrudServiceAsync<T> : ICrudServiceAsync<T>
        where T : Product
    {
        // Thread-safe dictionary for elements
        private readonly ConcurrentDictionary<Guid, T> _items = new();

        // Semaphore for asynchronous file save synchronization
        private readonly SemaphoreSlim _saveSemaphore = new(1, 1);

        // File path property
        public string FilePath { get; }

        // Constructor
        public CrudServiceAsync(string filePath = "products-lab2.json")
        {
            FilePath = filePath;
        }

        // Create element asynchronously
        public Task<bool> CreateAsync(T element)
        {
            // Validation
            ArgumentNullException.ThrowIfNull(element);

            // Thread-safe addition
            bool result = _items.TryAdd(element.Id, element);

            return Task.FromResult(result);
        }

        // Read element by ID asynchronously
        public Task<T> ReadAsync(Guid id)
        {
            // Try to get element by key
            _items.TryGetValue(id, out T? element);

            return Task.FromResult(element!);
        }

        // Read all elements asynchronously
        public Task<IEnumerable<T>> ReadAllAsync()
        {
            // Create a snapshot of values
            IEnumerable<T> result = _items.Values.ToList();

            return Task.FromResult(result);
        }

        // Read elements with pagination asynchronously
        public Task<IEnumerable<T>> ReadAllAsync(int page, int amount)
        {
            // Validation
            if (page < 1)
                throw new ArgumentOutOfRangeException(nameof(page));

            if (amount < 1)
                throw new ArgumentOutOfRangeException(nameof(amount));

            // Pagination query
            IEnumerable<T> result = _items.Values
                .OrderBy(x => x.Id)
                .Skip((page - 1) * amount)
                .Take(amount)
                .ToList();

            return Task.FromResult(result);
        }

        // Update element asynchronously
        public Task<bool> UpdateAsync(T element)
        {
            // Validation
            ArgumentNullException.ThrowIfNull(element);

            // Check if element exists
            if (!_items.TryGetValue(element.Id, out T? oldElement))
                return Task.FromResult(false);

            // Thread-safe update
            bool result = _items.TryUpdate(
                element.Id,
                element,
                oldElement);

            return Task.FromResult(result);
        }

        // Remove element asynchronously
        public Task<bool> RemoveAsync(T element)
        {
            // Validation
            ArgumentNullException.ThrowIfNull(element);

            // Thread-safe removal
            bool result = _items.TryRemove(
                element.Id,
                out _);

            return Task.FromResult(result);
        }

        // Save elements to JSON file asynchronously
        public async Task<bool> SaveAsync()
        {
            // Wait for lock
            await _saveSemaphore.WaitAsync();

            try
            {
                // Snapshot collection
                List<T> snapshot = _items.Values.ToList();

                // Serialize to JSON
                string json = JsonSerializer.Serialize(
                    snapshot,
                    new JsonSerializerOptions
                    {
                        WriteIndented = true
                    });

                // Path processing
                string fullPath = Path.GetFullPath(FilePath);

                string? directory = Path.GetDirectoryName(fullPath);

                // Ensure directory exists
                if (!string.IsNullOrWhiteSpace(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // Write to file asynchronously
                await File.WriteAllTextAsync(fullPath, json);

                return true;
            }
            finally
            {
                // Release lock
                _saveSemaphore.Release();
            }
        }

        // Get typed enumerator (IEnumerable implementation)
        public IEnumerator<T> GetEnumerator()
        {
            return _items.Values
                .ToList()
                .GetEnumerator();
        }

        // Get non-typed enumerator
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}