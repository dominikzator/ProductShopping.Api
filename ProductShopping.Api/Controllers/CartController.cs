using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductShopping.Application.Features.CartItem.Commands.AddCartItem;
using ProductShopping.Application.Features.CartItem.Commands.RemoveCartItem;
using ProductShopping.Application.Features.CartItem.Commands.RemoveCartItems;
using ProductShopping.Application.Features.CartItem.Queries.GetCartItemDetails;
using ProductShopping.Application.Features.CartItem.Queries.GetCartItems;
using ProductShopping.Application.Models.Paging;
using ProductShopping.Identity.Constants;

namespace ProductShopping.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = RoleNames.User)]
public class CartController(IMediator mediator) : BaseApiController
{
    /// <summary>
    /// Returns all Cart Items from User Cart. Can be called only by authenticated User.
    /// </summary>
    /// <param name="paginationParameters"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<PagedResult<CartItemDto>>> GetCartItems([FromQuery] PaginationParameters paginationParameters)
    {
        var cartItemsResult = await mediator.Send(new GetCartItemListQuery { PaginationParameters = paginationParameters });

        return ToActionResult(cartItemsResult);
    }

    /// <summary>
    /// Returns a Cart Item from User Cart with a given Cart Item ID. Can be called only by authenticated User.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<CartItemDto>> GetCartItem(int id)
    {
        var cartItemResult = await mediator.Send(new GetCartItemDetailQuery { Id = id });

        return ToActionResult(cartItemResult);
    }

    /// <summary>
    /// Adds a given Product to a User Cart. Can be called only by authenticated User.
    /// </summary>
    /// <param name="cartItemDto"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult<CartItemDto>> Post(AddCartItemCommand cartItem)
    {
        var addCartItemResult = await mediator.Send(cartItem);

        return ToActionResult(addCartItemResult);
    }

    /// <summary>
    /// Deletes a given Product from a User Cart. Can be called only by authenticated User.
    /// </summary>
    /// <param name="cartItemDto"></param>
    /// <returns></returns>
    [HttpDelete]
    public async Task<ActionResult> Remove(RemoveCartItemCommand cartItem)
    {
        var removeCartItemResult = await mediator.Send(cartItem);

        return ToActionResult(removeCartItemResult);
    }

    /// <summary>
    /// Deletes all Products from a User Cart. Can be called only by authenticated User.
    /// </summary>
    /// <returns></returns>
    [HttpDelete("clear")]
    public async Task<ActionResult> RemoveAll(RemoveCartItemsCommand cartItems)
    {
        var clearCartResult = await mediator.Send(cartItems);

        return ToActionResult(clearCartResult);
    }
}
