using FluentValidation.Results;
using ProductShopping.Application.Constants;

namespace ProductShopping.Application.Exceptions;

public class BadRequestException : ErrorCodeException
{
    public BadRequestException(string message) : base(message, ErrorCodes.BadRequest)
    {

    }
}