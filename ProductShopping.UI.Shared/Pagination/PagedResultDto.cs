namespace ProductShopping.UI.Shared.Pagination;

public sealed class PagedResultDto<T>
{
    public IEnumerable<T> Data { get; set; } = [];
    public PaginationMetadataDto Metadata { get; set; } = new();
}
