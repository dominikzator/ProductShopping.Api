using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductShopping.Application.Contracts;
using ProductShopping.Application.Contracts.Persistence;
using ProductShopping.Application.Features.Product.Commands.CreateProduct;
using ProductShopping.Application.Features.Product.Commands.DeleteProduct;
using ProductShopping.Application.Features.Product.Commands.UpdateProduct;
using ProductShopping.Application.Features.Product.Queries.GetProductDetails;
using ProductShopping.Application.Features.Product.Queries.GetProducts;
using ProductShopping.Application.Models.Filtering;
using ProductShopping.Application.Models.Paging;
using ProductShopping.Application.Results;
using ProductShopping.Identity.Constants;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProductShopping.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController(IMediator mediator, IProductsRepository productsRepository) : BaseApiController
    {
        /// <summary>
        /// Returns all Products in database paged by PaginationParameters and filtered by ProductFilterParameters. Can be called without authentication.
        /// </summary>
        /// <param name="paginationParameters"></param>
        /// <param name="productFilters"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<PagedResult<ProductDto>>> GetProducts([FromQuery] ProductFilterParameters productFilters, [FromQuery] PaginationParameters paginationParameters)
        {
            var productsResult = await mediator.Send(new GetProductListQuery{ PaginationParameters = paginationParameters, ProductFilterParameters = productFilters });

            return ToActionResult(productsResult);
        }

        /// <summary>
        /// Returns a Product with a given Product ID. Can be called without authentication.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            var productResult = await mediator.Send(new GetProductDetailQuery { Id = id });
            
            return ToActionResult(productResult);
        }

        [HttpGet("categories")]
        public async Task<ActionResult<List<string>>> GetCategories()
        {
            var categories = await productsRepository.GetCategoryNamesAsync();

            return Ok(categories);
        }

        /// <summary>
        /// Adds a Product to a database. Can be called only by Administrator.
        /// </summary>
        /// <param name="productDto"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = RoleNames.Administrator)]
        public async Task<ActionResult<ProductDto>> Post(CreateProductCommand product)
        {
            var createdProductDtoResult = await mediator.Send(product);

            return CreatedAtAction(nameof(GetProduct), new { id = createdProductDtoResult.Value!.Id }, createdProductDtoResult.Value);
        }

        /// <summary>
        /// Updates Product in a database. Can be called only by Administrator.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updateDto"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [Authorize(Roles = RoleNames.Administrator)]
        public async Task<ActionResult> Put(UpdateProductCommand product)
        {
            var updateResult = await mediator.Send<Result>(product);

            return ToActionResult(updateResult);
        }

        /// <summary>
        /// Deletes Product from database. Can be called only by Administrator.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = RoleNames.Administrator)]
        public async Task<ActionResult> Delete(int id)
        {
            var deleteResult = await mediator.Send(new DeleteProductCommand { Id = id });

            return ToActionResult(deleteResult);
        }
    }
}
