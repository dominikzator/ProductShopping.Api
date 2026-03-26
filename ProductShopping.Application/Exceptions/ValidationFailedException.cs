namespace ProductShopping.Application.Exceptions;

public class ValidationFailedException : Exception
{
    public ValidationFailedException(string message) : base(message)
    {

    }
}
