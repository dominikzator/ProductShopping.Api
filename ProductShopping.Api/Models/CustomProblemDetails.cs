using Microsoft.AspNetCore.Mvc;

namespace ProductShopping.Api.Models;

public class CustomProblemDetails : ProblemDetails
{
    public List<string> ErrorDetails { get; set; } = new List<string>();
}
