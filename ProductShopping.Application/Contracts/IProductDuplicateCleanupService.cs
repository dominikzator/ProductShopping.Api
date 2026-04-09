using ProductShopping.Domain.Models;

namespace ProductShopping.Api.Contracts
{
    public interface IProductDuplicateCleanupService
    {
        Task<IReadOnlyList<Product>> GetDuplicateProductsToRemoveAsync(CancellationToken cancellationToken = default);
        Task<int> RemoveDuplicatesAsync(CancellationToken cancellationToken = default);
    }
}