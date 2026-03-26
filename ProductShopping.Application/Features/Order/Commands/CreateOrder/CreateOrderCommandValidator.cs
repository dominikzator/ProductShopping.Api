using FluentValidation;

namespace ProductShopping.Application.Features.Order.Commands.CreateOrder;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(o => o.Address.Street).MaximumLength(30).WithMessage("Address Street must not contain more than 30 characters.");
        RuleFor(o => o.Address.BuildingNumber).NotEmpty().MaximumLength(4).WithMessage("Address Building Number must not contain more than 4 numbers.");
        RuleFor(o => o.Address.ApartmentNumber).MaximumLength(3).WithMessage("Address Apartment Number must not contain more than 3 numbers.");
        RuleFor(o => o.Address.City).NotEmpty().MaximumLength(50).WithMessage("Address City must not contain more than 50 characters.");
        RuleFor(o => o.Address.PostalCode).NotEmpty().MaximumLength(20).WithMessage("Address Postal Code must not contain more than 20 characters.");
        RuleFor(o => o.Address.Country).NotEmpty().MaximumLength(20).WithMessage("Address Country must not contain more than 20 characters.");
        RuleFor(o => o.Address.PhoneNumber).NotEmpty().MaximumLength(20).WithMessage("Address Phone Number must not contain more than 20 numbers.");
    }
}