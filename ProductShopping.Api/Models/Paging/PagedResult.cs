using Elfie.Serialization;
using ProductShopping.Api.DTOs.CartItem;

namespace ProductShopping.Api.Models.Paging;

public class PagedResult<T>
{
    public static PaginationMetadata GetPaginationMetadata(IEnumerable<T> source, PaginationParameters paginationParameters)
    {
        var totalCount = source.Count();

        var totalPages = (int)Math.Ceiling(totalCount / (double)paginationParameters.PageSize);

        var metadata = new PaginationMetadata
        {
            CurrentPage = paginationParameters.PageNumber,
            PageSize = paginationParameters.PageSize,
            TotalCount = totalCount,
            TotalPages = totalPages,
            HasNext = paginationParameters.PageNumber < totalPages,
            HasPrevious = paginationParameters.PageNumber > 1
        };

        return metadata;
    }

    public static List<T> GetData(IEnumerable<T> source, PaginationParameters paginationParameters) => source
            .Skip((paginationParameters.PageNumber - 1) * paginationParameters.PageSize)
            .Take(paginationParameters.PageSize)
            .ToList();

    public IEnumerable<T> Data { get; set; } = [];
    public PaginationMetadata Metadata { get; set; } = new();
}
