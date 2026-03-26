using MediatR;
using ProductShopping.Application.Results;

namespace ProductShopping.Application.Features.Order.Commands.DeleteOrder;

public class DeleteOrderCommand : IRequest<Result>
{
    public int Id { get; set; }
}