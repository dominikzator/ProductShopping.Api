using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductShopping.Api.Contracts;
using ProductShopping.Domain.Models;
using ProductShopping.Persistence.DatabaseContext;

namespace ProductShopping.Infrastructure.Services;

public sealed class ProductDuplicateCleanupService : IProductDuplicateCleanupService
{
    private readonly ProductShoppingDbContext _dbContext;
    private readonly ILogger<ProductDuplicateCleanupService> _logger;

    public ProductDuplicateCleanupService(
        ProductShoppingDbContext dbContext,
        ILogger<ProductDuplicateCleanupService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<IReadOnlyList<Product>> GetDuplicateProductsToRemoveAsync(
        CancellationToken cancellationToken = default)
    {
        var products = await _dbContext.Products
            .OrderBy(p => p.Name)
            .ThenBy(p => p.Id)
            .ToListAsync(cancellationToken);

        var duplicatesToRemove = products
            .GroupBy(p => p.Name)
            .SelectMany(g => g.Skip(1))
            .ToList();

        return duplicatesToRemove;
    }

    public async Task<int> RemoveDuplicatesAsync(CancellationToken cancellationToken = default)
    {
        var duplicatesToRemove = await GetDuplicateProductsToRemoveAsync(cancellationToken);

        if (duplicatesToRemove.Count == 0)
        {
            _logger.LogInformation("No duplicate products found.");
            return 0;
        }

        foreach (var product in duplicatesToRemove)
        {
            _logger.LogWarning(
                "Removing duplicate product: Id={Id}, Name={Name}",
                product.Id,
                product.Name);
        }

        _dbContext.Products.RemoveRange(duplicatesToRemove);
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
