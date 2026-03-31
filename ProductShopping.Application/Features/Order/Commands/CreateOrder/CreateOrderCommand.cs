using MediatR;
using ProductShopping.Application.Features.Order.Queries.GetOrderDetails;
using ProductShopping.Application.Results;
using ProductShopping.Domain.Models;

namespace ProductShopping.Application.Features.Order.Commands.CreateOrder;

public class CreateOrderCommand : IRequest<Result<OrderDto>>
{
    public required Address Address { get; set; }
}