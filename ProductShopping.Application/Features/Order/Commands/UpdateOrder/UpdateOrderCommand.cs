using MediatR;
using ProductShopping.Application.Features.Order.Commands.CreateOrder;
using ProductShopping.Application.Results;

namespace ProductShopping.Application.Features.Order.Commands.UpdateOrder;

public class UpdateOrderCommand : CreateOrderCommand, IRequest<Result>
{
    public int Id { get; set; }
}