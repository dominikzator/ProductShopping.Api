using Microsoft.EntityFrameworkCore;
using ProductShopping.Application.Models.Paging;

namespace ProductShopping.Persistence.Extensions;

public static class QueryableExtensions
{
    public static async Task<(List<T> Items, int TotalCount, int TotalPages)> ToPagedDataAsync<T>(
    this IQueryable<T> source, PaginationParameters paginationParameters)
    {
        var totalCount = await source.CountAsync();
        var items = await source
            .Skip((paginationParameters.PageNumber - 1) * paginationParameters.PageSize)
            .Take(paginationParameters.PageSize)
            .ToListAsync();

        var totalPages = (int)Math.Ceiling(totalCount / (double)paginationParameters.PageSize);

        return (items, totalCount, totalPages);
    }
}
