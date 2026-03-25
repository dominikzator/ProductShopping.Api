using HR.LeaveManagement.Api.Models;
using HR.LeaveManagement.Application.Contracts.Logging;
using HR.LeaveManagement.Application.Exceptions;
using Newtonsoft.Json;
using System.Net;

namespace HR.LeaveManagement.Api.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IAppLogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, IAppLogger<ExceptionMiddleware> logger)
        {
            _next = next;
            this._logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext httpContext, Exception ex)
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
                        Errors = badRequestException.ValidationErrors
                    };
                    break;
                case NotFoundException NotFound:
                    statusCode = HttpStatusCode.NotFound;
                    problem = new CustomProblemDetails
                    {
                        Title = NotFound.Message,
                        Status = (int)statusCode,
                        Type = nameof(NotFoundException),
                        Detail = NotFound.InnerException?.Message,
                    };
                    break;
                case TestingException testingException:
                    statusCode = HttpStatusCode.Forbidden;
                    problem = new CustomProblemDetails
                    {
                        Title = "This is Testing Exception Title",
                        Status = (int)statusCode,
                        Type = nameof(TestingException),
                        Detail = "This is Testing Exception Detail",
                        Errors = new Dictionary<string, string[]>
                        {
                            ["Errors"] = new[] { "Name is required", "Email is invalid" },
                            ["Warnings"] = new[] { "Password is weak" }
                        }
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
            _logger.LogError(logMessage);
            await httpContext.Response.WriteAsJsonAsync(problem);

        }
    }
}