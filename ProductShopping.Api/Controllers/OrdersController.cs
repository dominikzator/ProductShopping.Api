using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductShopping.Api.Constants;
using ProductShopping.Api.Contracts;
using ProductShopping.Api.DTOs.CartItem;
using ProductShopping.Api.DTOs.Order;
using ProductShopping.Api.Models.Paging;
using ProductShopping.Api.Services;

namespace ProductShopping.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrdersController(IOrdersService ordersService) : BaseApiController
{
    [HttpGet]
    [Authorize(Roles = RoleNames.User)]
    public async Task<ActionResult<PagedResult<GetOrderDto>>> GetOrders([FromQuery] PaginationParameters paginationParameters)
    {
        var result = await ordersService.GetOrdersAsync(paginationParameters);

        return ToActionResult(result);
    }

    [HttpGet("{orderId}")]
    [Authorize(Roles = RoleNames.User)]
    public async Task<ActionResult<GetOrderDto>> GetOrder(int orderId)
    {
        var result = await ordersService.GetOrderAsync(orderId);

        return ToActionResult(result);
    }

    [HttpGet("{orderId}/orderItems")]
    [Authorize(Roles = RoleNames.User)]
    public async Task<ActionResult<PagedResult<GetOrderItemDto>>> GetOrderItems(int orderId, [FromQuery] PaginationParameters paginationParameters)
    {
        var result = await ordersService.GetOrderItemsAsync(orderId, paginationParameters);

        return ToActionResult(result);
    }

    [HttpGet("orderItems/{orderItemId}")]
    [Authorize(Roles = RoleNames.User)]
    public async Task<ActionResult<GetOrderItemDto>> GetOrderItem(int orderItemId)
    {
        var result = await ordersService.GetOrderItemAsync(orderItemId);

        return ToActionResult(result);
    }

    [HttpPost]
    [Authorize(Roles = RoleNames.User)]
    public async Task<ActionResult<GetOrderDto>> Post(CreateOrderDto createOrderDto)
    {
        var result = await ordersService.CreateOrder(createOrderDto);

        if (!result.IsSuccess) return MapErrorsToResponse(result.Errors);

        return ToActionResult(result);
    }

    [HttpDelete("{orderId}")]
    [Authorize(Roles = RoleNames.Administrator)]
    public async Task<ActionResult> DeleteOrder(int orderId)
    {
        var result = await ordersService.DeleteOrderAsync(orderId);

        return ToActionResult(result);
    }

    [HttpDelete("orderItems/{orderItemId}")]
    [Authorize(Roles = RoleNames.Administrator)]
    public async Task<ActionResult> DeleteOrderItem(int orderItemId)
    {
        var result = await ordersService.DeleteOrderItemAsync(orderItemId);

        return ToActionResult(result);
    }
}
