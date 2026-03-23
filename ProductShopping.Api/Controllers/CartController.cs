using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductShopping.Application.Contracts;
using ProductShopping.Application.DTOs.CartItem;
using ProductShopping.Application.Models.Paging;
using ProductShopping.Identity.Constants;

namespace ProductShopping.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = RoleNames.User)]
public class CartController(ICartItemsService cartItemsService) : BaseApiController
{
    /// <summary>
    /// Returns all Cart Items from User Cart. Can be called only by authenticated User.
    /// </summary>
    /// <param name="paginationParameters"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<PagedResult<GetCartItemDto>>> GetCartItems([FromQuery] PaginationParameters paginationParameters)
    {
        var result = await cartItemsService.GetCartItemsAsync(paginationParameters);

        return ToActionResult(result);
    }

    /// <summary>
    /// Returns a Cart Item from User Cart with a given Cart Item ID. Can be called only by authenticated User.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<GetCartItemDto>> GetCartItem(int id)
    {
        var result = await cartItemsService.GetCartItemAsync(id);

        return ToActionResult(result);
    }

    /// <summary>
    /// Adds a given Product to a User Cart. Can be called only by authenticated User.
    /// </summary>
    /// <param name="cartItemDto"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult<GetCartItemDto>> Post(CreateCartItemDto cartItemDto)
    {
        var result = await cartItemsService.AddCartItemToCartAsync(cartItemDto);
        if (!result.IsSuccess) return MapErrorsToResponse(result.Errors);

        return ToActionResult(result);
    }

    /// <summary>
    /// Deletes a given Product from a User Cart. Can be called only by authenticated User.
    /// </summary>
    /// <param name="cartItemDto"></param>
    /// <returns></returns>
    [HttpDelete]
    public async Task<ActionResult> Delete(RemoveCartItemDto cartItemDto)
    {
        var result = await cartItemsService.DeleteCartItemFromCartAsync(cartItemDto);

        return ToActionResult(result);
    }

    /// <summary>
    /// Deletes all Products from a User Cart. Can be called only by authenticated User.
    /// </summary>
    /// <returns></returns>
    [HttpDelete("clear")]
    public async Task<ActionResult> DeleteAll()
    {
        var result = await cartItemsService.ClearCartAsync();

        return ToActionResult(result);
    }
}
