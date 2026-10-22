using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineShop.Common.Repositories
{
    // Defines the asynchronous data operations used by the generic CRUD service.
    public interface IRepository<T> where T : class
    {
        Task<T> GetByIdAsync(int id);
        Task<T?> GetByGuidIdAsync(Guid id);
        Task<IEnumerable<T>> GetAllAsync();
        Task AddAsync(T entity);
        Task Update(T entity);
        Task Delete(T entity);
        Task<int> SaveChangesAsync();
    }
}
