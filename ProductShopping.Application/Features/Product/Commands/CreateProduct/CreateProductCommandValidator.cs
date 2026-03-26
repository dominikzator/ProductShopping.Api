using FluentValidation;
using ProductShopping.Application.Contracts.Persistence;

namespace ProductShopping.Application.Features.Product.Commands.CreateProduct;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    private readonly IProductsRepository _productsRepository;

    public CreateProductCommandValidator(IProductsRepository productsRepository)
    {
        _productsRepository = productsRepository;

        RuleFor(p => p.Name).NotEmpty().WithMessage("Name can't be empty");
        RuleFor(p => p.Name).MustAsync(ProductWithNameMustNotExist).WithMessage($"A product with the same Name already exists.");

        RuleFor(p => p.CategoryName).NotEmpty().WithMessage("CategoryName can't be empty.");
        RuleFor(p => p.CategoryName).MustAsync(CategoryNameMustExist).WithMessage("CategoryName does not exist.");

        RuleFor(p => p.Rating).GreaterThanOrEqualTo(1).LessThanOrEqualTo(5).WithMessage("Rating must be in range from 1 to 5.");

        RuleFor(p => p.Price).GreaterThan(0).WithMessage("Price must be greater than 0.");
    }
    private async Task<bool> ProductWithNameMustNotExist(string name, CancellationToken cancellationToken)
    {
        var product = await _productsRepository.GetProductByNameAsync(name);

        return product == null;
    }
    private async Task<bool> CategoryNameMustExist(string categoryName, CancellationToken cancellationToken)
    {
        var category = await _productsRepository.GetCategoryFromNameAsync(categoryName);

        return category != null;
    }
}