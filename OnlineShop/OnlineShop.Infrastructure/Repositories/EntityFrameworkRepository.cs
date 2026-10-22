using Microsoft.EntityFrameworkCore;
using OnlineShop.Common.Repositories;

namespace OnlineShop.Infrastructure.Repositories
{
    // Implements generic repository operations with an Entity Framework Core context.
    public class EntityFrameworkRepository<T> : IRepository<T> where T : class
    {
        private readonly OnlineShopContext _context;
        private readonly DbSet<T> _set;

        public EntityFrameworkRepository(OnlineShopContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _set = _context.Set<T>();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            var entityType = _context.Model.FindEntityType(typeof(T));
            var keyType = entityType?.FindPrimaryKey()?.Properties.FirstOrDefault()?.ClrType;

            if (keyType != typeof(int))
            {
                return null!;
            }

            return (await _set.FindAsync(id))!;
        }

        public async Task<T?> GetByGuidIdAsync(Guid id)
        {
            var entityType = _context.Model.FindEntityType(typeof(T));
            var keyType = entityType?.FindPrimaryKey()?.Properties.FirstOrDefault()?.ClrType;

            if (keyType != typeof(Guid))
            {
                return null;
            }

            return await _set.FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _set.AsNoTracking().ToListAsync();
        }

        public async Task AddAsync(T entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            await _set.AddAsync(entity);
        }

        public async Task Update(T entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            var idProperty = entity.GetType().GetProperty("Id");
            if (idProperty?.PropertyType == typeof(Guid) && idProperty.GetValue(entity) is Guid id)
            {
                var currentEntity = await GetByGuidIdAsync(id);
                if (currentEntity is not null && !ReferenceEquals(currentEntity, entity))
                {
                    _context.Entry(currentEntity).CurrentValues.SetValues(entity);
                    return;
                }
            }

            _set.Update(entity);
        }

        public Task Delete(T entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            _set.Remove(entity);
            return Task.CompletedTask;
        }

        public Task<int> SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
