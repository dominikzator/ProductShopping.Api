using FluentValidation;

namespace ProductShopping.Application.Features.CartItem.Commands.AddCartItem;

public class AddCartItemCommandValidator : AbstractValidator<AddCartItemCommand>
{
    public AddCartItemCommandValidator()
    {
        RuleFor(c => c.Quantity).GreaterThan(0).LessThanOrEqualTo(1000).WithMessage("Quantity must be in range from 1 to 1000");
    }
}