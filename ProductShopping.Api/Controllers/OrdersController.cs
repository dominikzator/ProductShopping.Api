using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductShopping.Application.Features.Order.Commands.CreateOrder;
using ProductShopping.Application.Features.Order.Commands.DeleteOrder;
using ProductShopping.Application.Features.Order.Commands.UpdateOrder;
using ProductShopping.Application.Features.Order.Queries.GetOrderDetails;
using ProductShopping.Application.Features.Order.Queries.GetOrders;
using ProductShopping.Application.Models.Paging;
using ProductShopping.Application.Results;
using ProductShopping.Identity.Constants;

namespace ProductShopping.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrdersController(IMediator mediator) : BaseApiController
{
    /// <summary>
    /// Returns all User Orders. Can be called only by authenticated User.
    /// </summary>
    /// <param name="paginationParameters"></param>
    /// <returns></returns>
    [HttpGet]
    [Authorize(Roles = RoleNames.User)]
    public async Task<ActionResult<PagedResult<OrderDto>>> GetOrders([FromQuery] PaginationParameters paginationParameters)
    {
        var ordersResult = await mediator.Send(new GetOrderListQuery { PaginationParameters = paginationParameters});

        return ToActionResult(ordersResult);
    }

    /// <summary>
    /// Returns a User Order with a given Order ID. Can be called only by authenticated User.
    /// </summary>
    /// <param name="orderId"></param>
    /// <returns></returns>
    [HttpGet("{orderId}")]
    [Authorize(Roles = RoleNames.User)]
    public async Task<ActionResult<OrderDto>> GetOrder(int orderId)
    {
        var orderResult = await mediator.Send(new GetOrderDetailQuery
        {
            Id = orderId
        });

        return ToActionResult(orderResult);
    }

    /// <summary>
    /// Creates an Order from items in a User Cart and takes Address parameter. Can be called only by authenticated User.
    /// </summary>
    /// <param name="createOrderDto"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize(Roles = RoleNames.User)]
    public async Task<ActionResult<OrderDto>> Post(CreateOrderCommand order)
    {
        var createOrderResult = await mediator.Send(order);

        return ToActionResult(createOrderResult);
    }

    /// <summary>
    /// Updates an Order. Can be called only by an Administrator.
    /// </summary>
    /// <param name="createOrderDto"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    [Authorize(Roles = RoleNames.Administrator)]
    public async Task<ActionResult<OrderDto>> Put(UpdateOrderCommand order)
    {
        var updateOrderResult = await mediator.Send<Result>(order);

        return ToActionResult(updateOrderResult);
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
        var deleteOrderResult = await mediator.Send(new DeleteOrderCommand
        {
            Id = orderId
        });

        return ToActionResult(deleteOrderResult);
    }
}
