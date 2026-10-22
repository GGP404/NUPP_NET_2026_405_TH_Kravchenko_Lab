using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using OnlineShop.Common.Repositories;

namespace OnlineShop.Common.Services
{
    public class CrudServiceAsync<T> : ICrudServiceAsync<T>
        where T : class
    {
        // Keeps the original in-memory storage mode available for earlier laboratory work.
        private readonly ConcurrentDictionary<Guid, T> _items = new();

        // Uses the repository when the service is configured for database access.
        private readonly IRepository<T>? _repository;

        // Serializes asynchronous writes to the original JSON storage.
        private readonly SemaphoreSlim _saveSemaphore = new(1, 1);

        // Exposes the file path used by the original JSON storage mode.
        public string FilePath { get; }

        // Creates a service that uses the original JSON-oriented storage behavior.
        public CrudServiceAsync(string filePath = "products-lab2.json")
        {
            FilePath = filePath;
        }

        // Creates a service that delegates data operations to an injected repository.
        public CrudServiceAsync(IRepository<T> repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            FilePath = string.Empty;
        }

        // Adds an entity to the selected storage mode.
        public async Task<bool> CreateAsync(T element)
        {
            ArgumentNullException.ThrowIfNull(element);

            if (_repository is not null)
            {
                Guid id = GetEntityId(element);
                if (await _repository.GetByGuidIdAsync(id) is not null)
                    return false;

                await _repository.AddAsync(element);
                await SaveAsync();
                return true;
            }

            return _items.TryAdd(GetEntityId(element), element);
        }

        // Reads a single entity by its GUID identifier.
        public async Task<T> ReadAsync(Guid id)
        {
            if (_repository is not null)
            {
                return (await _repository.GetByGuidIdAsync(id))!;
            }

            _items.TryGetValue(id, out T? element);
            return element!;
        }

        // Reads all entities from the selected storage mode.
        public async Task<IEnumerable<T>> ReadAllAsync()
        {
            if (_repository is not null)
            {
                return await _repository.GetAllAsync();
            }

            return _items.Values.ToList();
        }

        // Reads a page of entities ordered by their GUID identifiers.
        public async Task<IEnumerable<T>> ReadAllAsync(int page, int amount)
        {
            if (page < 1)
                throw new ArgumentOutOfRangeException(nameof(page));

            if (amount < 1)
                throw new ArgumentOutOfRangeException(nameof(amount));

            IEnumerable<T> source = _repository is not null
                ? await _repository.GetAllAsync()
                : _items.Values.ToList();

            return source
                .OrderBy(GetEntityId)
                .Skip((page - 1) * amount)
                .Take(amount)
                .ToList();
        }

        // Updates an existing entity in the selected storage mode.
        public async Task<bool> UpdateAsync(T element)
        {
            ArgumentNullException.ThrowIfNull(element);
            Guid id = GetEntityId(element);

            if (_repository is not null)
            {
                T? existing = await _repository.GetByGuidIdAsync(id);
                if (existing is null)
                    return false;

                await _repository.Update(element);
                await SaveAsync();
                return true;
            }

            if (!_items.TryGetValue(id, out T? oldElement))
                return false;

            return _items.TryUpdate(id, element, oldElement);
        }

        // Removes an existing entity from the selected storage mode.
        public async Task<bool> RemoveAsync(T element)
        {
            ArgumentNullException.ThrowIfNull(element);
            Guid id = GetEntityId(element);

            if (_repository is not null)
            {
                T? existing = await _repository.GetByGuidIdAsync(id);
                if (existing is null)
                    return false;

                await _repository.Delete(existing);
                await SaveAsync();
                return true;
            }

            return _items.TryRemove(id, out _);
        }

        // Saves changes to the repository or writes the in-memory collection to JSON.
        public async Task<bool> SaveAsync()
        {
            if (_repository is not null)
            {
                await _repository.SaveChangesAsync();
                return true;
            }

            await _saveSemaphore.WaitAsync();

            try
            {
                List<T> snapshot = _items.Values.ToList();
                string json = JsonSerializer.Serialize(
                    snapshot,
                    new JsonSerializerOptions
                    {
                        WriteIndented = true
                    });

                string fullPath = Path.GetFullPath(FilePath);
                string? directory = Path.GetDirectoryName(fullPath);

                if (!string.IsNullOrWhiteSpace(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                await File.WriteAllTextAsync(fullPath, json);
                return true;
            }
            finally
            {
                _saveSemaphore.Release();
            }
        }

        // Enumerates entities from either the repository or the original in-memory collection.
        public IEnumerator<T> GetEnumerator()
        {
            if (_repository is not null)
            {
                return _repository.GetAllAsync()
                    .GetAwaiter()
                    .GetResult()
                    .GetEnumerator();
            }

            return _items.Values.ToList().GetEnumerator();
        }

        // Provides the non-generic enumerator required by IEnumerable.
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        // Reads the GUID key from entities that expose an Id property.
        private static Guid GetEntityId(T element)
        {
            var idProperty = element.GetType().GetProperty("Id");
            if (idProperty?.PropertyType != typeof(Guid) || idProperty.GetValue(element) is not Guid id)
            {
                throw new InvalidOperationException(
                    $"Type '{element.GetType().Name}' must expose a GUID Id property.");
            }

            return id;
        }
    }
}
