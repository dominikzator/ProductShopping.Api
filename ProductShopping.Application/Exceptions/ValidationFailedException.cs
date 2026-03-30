using FluentValidation.Results;
using ProductShopping.Application.Constants;

namespace ProductShopping.Application.Exceptions;

public class ValidationFailedException : ErrorCodeException
{
    public ValidationResult ValidationResult { get; }
    public List<ValidationFailure> Errors { get; }
    public string InvalidFieldName { get; }

    public ValidationFailedException(string message, ValidationResult validationResult) : base(message, ErrorCodes.Validation)
    {
        ValidationResult = validationResult;
        Errors = ValidationResult.Errors;
        InvalidFieldName = Errors[0].PropertyName;
    }
}