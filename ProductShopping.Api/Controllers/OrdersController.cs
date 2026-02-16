using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductShopping.Api.Constants;
using ProductShopping.Api.Contracts;
using ProductShopping.Api.DTOs.CartItem;
using ProductShopping.Api.DTOs.Order;

namespace ProductShopping.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrdersController(IOrdersService ordersService) : BaseApiController
{
    [HttpPost]
    [Authorize(Roles = RoleNames.User)]
    public async Task<ActionResult<GetOrderDto>> Post(CreateOrderDto createOrderDto)
    {
        var result = await ordersService.CreateOrder(createOrderDto);

        if (!result.IsSuccess) return MapErrorsToResponse(result.Errors);

        return ToActionResult(result);
    }
}
