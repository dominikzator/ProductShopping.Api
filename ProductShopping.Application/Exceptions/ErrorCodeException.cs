namespace ProductShopping.Application.Exceptions;

public class ErrorCodeException : Exception
{
    public string ErrorCode { get; }
    public string Message { get; }

    public ErrorCodeException(string message, string errorCode) : base(message)
    {
        ErrorCode = errorCode;
        Message = message;
    }
}
