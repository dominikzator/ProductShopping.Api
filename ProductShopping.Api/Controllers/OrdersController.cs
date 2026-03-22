using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductShopping.Api.Constants;
using ProductShopping.Api.Contracts;
using ProductShopping.Api.DTOs.Order;
using ProductShopping.Application.Models.Paging;
using ProductShopping.Identity.Constants;

namespace ProductShopping.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrdersController(IOrdersService ordersService, ILogger<OrdersController> logger, IHttpContextAccessor httpContextAccessor) : BaseApiController
{
    /// <summary>
    /// Returns all User Orders. Can be called only by authenticated User.
    /// </summary>
    /// <param name="paginationParameters"></param>
    /// <returns></returns>
    [HttpGet]
    [Authorize(Roles = RoleNames.User)]
    public async Task<ActionResult<PagedResult<GetOrderDto>>> GetOrders([FromQuery] PaginationParameters paginationParameters)
    {
        var result = await ordersService.GetOrdersAsync(paginationParameters);

        return ToActionResult(result);
    }

    /// <summary>
    /// Returns a User Order with a given Order ID. Can be called only by authenticated User.
    /// </summary>
    /// <param name="orderId"></param>
    /// <returns></returns>
    [HttpGet("{orderId}")]
    [Authorize(Roles = RoleNames.User)]
    public async Task<ActionResult<GetOrderDto>> GetOrder(int orderId)
    {
        var result = await ordersService.GetOrderAsync(orderId);

        return ToActionResult(result);
    }

    /// <summary>
    /// Returns all Order Items of an Order with a given Order ID. Can be called only by authenticated User.
    /// </summary>
    /// <param name="orderId"></param>
    /// <param name="paginationParameters"></param>
    /// <returns></returns>
    [HttpGet("{orderId}/orderItems")]
    [Authorize(Roles = RoleNames.User)]
    public async Task<ActionResult<PagedResult<GetOrderItemDto>>> GetOrderItems(int orderId, [FromQuery] PaginationParameters paginationParameters)
    {
        var result = await ordersService.GetOrderItemsAsync(orderId, paginationParameters);

        return ToActionResult(result);
    }

    /// <summary>
    /// Returns an Order Item with a given Order Item ID. Can be called only by authenticated User.
    /// </summary>
    /// <param name="orderItemId"></param>
    /// <returns></returns>
    [HttpGet("orderItems/{orderItemId}")]
    [Authorize(Roles = RoleNames.User)]
    public async Task<ActionResult<GetOrderItemDto>> GetOrderItem(int orderItemId)
    {
        var result = await ordersService.GetOrderItemAsync(orderItemId);

        return ToActionResult(result);
    }

    /// <summary>
    /// Creates an Order from items in a User Cart and takes Address parameter. Can be called only by authenticated User.
    /// </summary>
    /// <param name="createOrderDto"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize(Roles = RoleNames.User)]
    public async Task<ActionResult<GetOrderDto>> Post(CreateOrderDto createOrderDto)
    {
        var result = await ordersService.CreateOrder(createOrderDto);

        if (!result.IsSuccess) return MapErrorsToResponse(result.Errors);

        return ToActionResult(result);
    }

    /// <summary>
    /// Deletes an Order with all associated OrderItems with a given order ID. Can be called only by an Administrator.
    /// </summary>
    /// <param name="orderId"></param>
    /// <returns></returns>
    [HttpDelete("{orderId}")]
    [Authorize(Roles = RoleNames.Administrator)]
    public async Task<ActionResult> DeleteOrder(int orderId)
    {
        var result = await ordersService.DeleteOrderAsync(orderId);

        return ToActionResult(result);
    }

    /// <summary>
    /// Deletes a single Order Item with a given Order Item ID. Can be called only by Administrator.
    /// </summary>
    /// <param name="orderItemId"></param>
    /// <returns></returns>
    [HttpDelete("orderItems/{orderItemId}")]
    [Authorize(Roles = RoleNames.Administrator)]
    public async Task<ActionResult> DeleteOrderItem(int orderItemId)
    {
        var result = await ordersService.DeleteOrderItemAsync(orderItemId);

        return ToActionResult(result);
    }
}
