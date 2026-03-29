using MediatR;
using ProductShopping.Application.Features.Product.Queries.GetProductDetails;
using ProductShopping.Application.Models.Filtering;
using ProductShopping.Application.Models.Paging;
using ProductShopping.Application.Results;

namespace ProductShopping.Application.Features.Product.Queries.GetProducts;

public class GetProductListQuery : IRequest<Result<PagedResult<ProductDto>>>
{
    public PaginationParameters? PaginationParameters { get; set; }
    public ProductFilterParameters? ProductFilterParameters { get; set; }
}