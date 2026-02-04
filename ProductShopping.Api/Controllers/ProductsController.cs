using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductShopping.Api.Constants;
using ProductShopping.Api.Contracts;
using ProductShopping.Api.DTOs.Product;
using ProductShopping.Api.Models.Filtering;
using ProductShopping.Api.Models.Paging;
using ProductShopping.Api.Results;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProductShopping.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController(IProductsService productsService) : BaseApiController
    {
        // GET: api/<ProductsController>
        [HttpGet]
        public async Task<ActionResult<PagedResult<GetProductDto>>> GetProducts([FromQuery] PaginationParameters paginationParameters, [FromQuery] ProductFilterParameters productFilters)
        {
            var result = await productsService.GetProductsAsync(paginationParameters, productFilters);

            return ToActionResult(result);
        }

        // GET api/<ProductsController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GetProductDto>> GetProduct(int id)
        {
            var result = await productsService.GetProductAsync(id);

            return ToActionResult(result);
        }

        // POST api/<ProductsController>
        [HttpPost]
        [Authorize(Roles = RoleNames.Administrator)]
        public async Task<ActionResult<GetProductDto>> Post(CreateProductDto productDto)
        {
            var result = await productsService.CreateProductAsync(productDto);
            if (!result.IsSuccess) return MapErrorsToResponse(result.Errors);

            return CreatedAtAction(nameof(GetProduct), new { id = result.Value!.Id }, result.Value);
        }

        // PUT api/<ProductsController>/5
        [HttpPut("{id}")]
        [Authorize(Roles = RoleNames.Administrator)]
        public async Task<ActionResult> Put(int id, UpdateProductDto updateDto)
        {
            var result = await productsService.UpdateProductAsync(id, updateDto);

            return ToActionResult(result);
        }

        // DELETE api/<ProductsController>/5
        [HttpDelete("{id}")]
        [Authorize(Roles = RoleNames.Administrator)]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await productsService.DeleteProductAsync(id);

            return ToActionResult(result);
        }
    }
}
