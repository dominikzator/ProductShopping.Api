using ProductShopping.Application.Results;
using ProductShopping.Domain.Common;
using ProductShopping.Domain.Models;

namespace ProductShopping.Application.Contracts.Persistence
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task CreateAsync(T entity);
        Task DeleteAsync(T entity);
        Task<IReadOnlyList<T>> GetAsync();
        Task<T?> GetByIdAsync(int? id, bool tracking = false);
        Task SaveChangesAsync();
        void SetEntityAsModified<TEntity>(TEntity entity) where TEntity : class;
        Task UpdateAsync(T entity);
    }
}