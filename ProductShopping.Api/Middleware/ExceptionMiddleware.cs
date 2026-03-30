using Microsoft.AspNetCore.Http.HttpResults;
using Newtonsoft.Json;
using ProductShopping.Api.Models;
using ProductShopping.Application.Contracts.Logging;
using ProductShopping.Application.Exceptions;
using System.Net;

namespace ProductShopping.Api.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext, IAppLogger<ExceptionMiddleware> logger)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex, logger);
            }
        }

        private async Task HandleExceptionAsync(HttpContext httpContext, Exception ex, IAppLogger<ExceptionMiddleware> logger)
        {
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
            CustomProblemDetails problem = new();

            switch (ex)
            {
                case BadRequestException badRequestException:
                    statusCode = HttpStatusCode.BadRequest;
                    problem = new CustomProblemDetails
                    {
                        Title = badRequestException.Message,
                        Status = (int)statusCode,
                        Detail = badRequestException.InnerException?.Message,
                        Type = nameof(BadRequestException),
                    };
                    break;
                case NotFoundException notFound:
                    statusCode = HttpStatusCode.NotFound;
                    problem = new CustomProblemDetails
                    {
                        Title = notFound.Message,
                        Status = (int)statusCode,
                        Type = nameof(NotFoundException),
                        Detail = notFound.InnerException?.Message,
                    };
                    break;
                case ValidationFailedException validationFailed:
                    statusCode = HttpStatusCode.UnprocessableEntity;
                    problem = new CustomProblemDetails
                    {
                        Title = validationFailed.Message,
                        Status = (int)statusCode,
                        Type = nameof(ValidationFailedException),
                        Detail = validationFailed.InnerException?.Message,
                        ErrorDetails = validationFailed.ValidationResult.Errors.Select(p => p.ErrorMessage).ToList()
                    };
                    break;
                default:
                    problem = new CustomProblemDetails
                    {
                        Title = ex.Message,
                        Status = (int)statusCode,
                        Type = nameof(HttpStatusCode.InternalServerError),
                        Detail = ex.StackTrace,
                    };
                    break;
            }

            httpContext.Response.StatusCode = (int)statusCode;
            var logMessage = JsonConvert.SerializeObject(problem);
            logger.LogError(logMessage);
            await httpContext.Response.WriteAsJsonAsync(problem);

        }
    }
}