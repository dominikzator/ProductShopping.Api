using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductShopping.Api.Constants;
using ProductShopping.Api.Contracts;
using ProductShopping.Api.DTOs.CartItem;
using ProductShopping.Api.DTOs.Product;
using ProductShopping.Api.Models.Filtering;
using ProductShopping.Api.Models.Paging;
using ProductShopping.Api.Services;

namespace ProductShopping.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = RoleNames.User)]
public class CartController(ICartItemsService cartItemsService) : BaseApiController
{
    // GET: api/<ProductsController>
    [HttpGet]
    public async Task<ActionResult<PagedResult<GetCartItemDto>>> GetCartItems([FromQuery] PaginationParameters paginationParameters)
    {
        var result = await cartItemsService.GetCartItemsAsync(paginationParameters);

        return ToActionResult(result);
    }

    // GET api/<ProductsController>/5
    [HttpGet("{id}")]
    public async Task<ActionResult<GetCartItemDto>> GetCartItem(int id)
    {
        var result = await cartItemsService.GetCartItemAsync(id);

        return ToActionResult(result);
    }

    // POST api/<CartController>
    [HttpPost]
    public async Task<ActionResult<GetCartItemDto>> Post(CreateCartItemDto cartItemDto)
    {
        var result = await cartItemsService.AddCartItemToCartAsync(cartItemDto);
        if (!result.IsSuccess) return MapErrorsToResponse(result.Errors);

        return ToActionResult(result);
    }

    // DELETE api/<CartController>/5
    [HttpDelete]
    public async Task<ActionResult> Delete(RemoveCartItemDto cartItemDto)
    {
        var result = await cartItemsService.DeleteCartItemFromCartAsync(cartItemDto);

        return ToActionResult(result);
    }

    [HttpDelete("api/[controller]/clear")]
    public async Task<ActionResult> DeleteAll()
    {
        var result = await cartItemsService.ClearCartAsync();

        return ToActionResult(result);
    }
}
