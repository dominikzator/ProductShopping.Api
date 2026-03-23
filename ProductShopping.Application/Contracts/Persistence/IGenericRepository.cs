using ProductShopping.Domain.Common;

namespace ProductShopping.Application.Contracts.Persistence
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task CreateAsync(T entity);
        Task DeleteAsync(T entity);
        Task<IReadOnlyList<T>> GetAsync();
        Task<T> GetByIdAsync(int id);
        Task UpdateAsync(T entity);
    }
}