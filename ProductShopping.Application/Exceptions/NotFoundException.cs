using ProductShopping.Application.Constants;

namespace ProductShopping.Application.Exceptions;

public class NotFoundException : ErrorCodeException
{
    public NotFoundException(string message) : base(message, ErrorCodes.NotFound) 
    { 
        
    }
}