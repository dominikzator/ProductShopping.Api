using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProductShopping.Application.Features.Order.Commands.CreateOrder;
using ProductShopping.Application.Models.Paging;
using ProductShopping.Domain.Models;
using ProductShopping.UI.Shared.Contracts;
using System.ComponentModel.DataAnnotations;

namespace ProductShopping.UI.RazorPagesUI.Pages;

public class CreateOrderModel(ICartsApiClient cartsApiClient, IOrdersApiClient ordersApiClient, IHttpContextAccessor httpContextAccessor) : PageModel
{
    public List<CartLineVm> Items { get; private set; } = [];
    public int TotalQuantity => Items.Sum(x => x.Quantity);
    public decimal Subtotal => Items.Sum(x => x.LineTotal);

    [BindProperty]
    public AddressInputModel Address { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(CancellationToken ct)
    {
        if (!User.Identity?.IsAuthenticated ?? true)
        {
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        }

        await LoadCartAsync(ct);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken ct)
    {
        Console.WriteLine("OnPostAsync CreateOrder!");

        if (!User.Identity?.IsAuthenticated ?? true)
        {
            return new JsonResult(new
            {
                success = false,
                message = "You need to sign in first."
            })
            {
                StatusCode = 401
            };
        }

        await LoadCartAsync(ct);

        if (!Items.Any())
        {
            return new JsonResult(new
            {
                success = false,
                message = "Your cart is empty."
            })
            {
                StatusCode = 400
            };
        }

        if (!ModelState.IsValid)
        {
            var firstError = ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .SelectMany(x => x.Value!.Errors.Select(e => e.ErrorMessage))
                .FirstOrDefault();

            return new JsonResult(new
            {
                success = false,
                message = firstError ?? "Something went wrong."
            })
            {
                StatusCode = 400
            };
        }

        var token = GetAccessToken();

        var request = httpContextAccessor.HttpContext?.Request;
        var baseUrl = $"{request?.Scheme}://{request?.Host}{request?.PathBase}";
        Console.WriteLine("baseUrl: " + baseUrl);

        var order = await ordersApiClient.CreateOrderAsync(token!, new CreateOrderCommand
        {
            Address = new Address
            {
                PhoneNumber = Address.PhoneNumber,
                ApartmentNumber = Address.ApartmentNumber,
                BuildingNumber = Address.BuildingNumber,
                City = Address.City,
                Country = Address.Country,
                PostalCode = Address.PostalCode,
                Street = Address.Street
            },
            DomainName = baseUrl
        }, ct);

        if (order is null || string.IsNullOrWhiteSpace(order.PaymentUrl))
        {
            return new JsonResult(new
            {
                success = false,
                message = "Order was created, but payment URL is missing."
            })
            {
                StatusCode = 500
            };
        }

        return new JsonResult(new
        {
            success = true,
            message = "Redirecting to payment page…",
            redirectUrl = order.PaymentUrl
        });
    }

    private async Task LoadCartAsync(CancellationToken ct)
    {
        var token = GetAccessToken();
        if (string.IsNullOrWhiteSpace(token))
        {
            Items = [];
            return;
        }

        var result = await cartsApiClient.GetCartItemsAsync(token, new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 100
        }, ct);

        Items = result?.Data?
            .Select(x => new CartLineVm
            {
                CartItemId = x.Id,
                ProductId = x.ProductId,
                ProductName = x.Name,
                CategoryName = x.CategoryName ?? "Product",
                ImageUrl = x.ImageUrl,
                Quantity = x.Quantity,
                UnitPrice = x.UnitPrice
            })
            .ToList() ?? [];
    }

    private string? GetAccessToken()
        => User.FindFirst("access_token")?.Value;

    public class AddressInputModel
    {
        [Required, MaxLength(100)]
        [Display(Name = "Street")]
        public string Street { get; set; } = "Milkyway street";

        [MaxLength(4)]
        [Display(Name = "Building number")]
        public string? BuildingNumber { get; set; } = "19";

        [MaxLength(3)]
        [Display(Name = "Apartment number")]
        public string? ApartmentNumber { get; set; } = "3";

        [Required, MaxLength(50)]
        [Display(Name = "City")]
        public string City { get; set; } = "Glasgow";

        [Required, MaxLength(20)]
        [Display(Name = "Postal code")]
        public string PostalCode { get; set; } = "12-345";

        [Required, MaxLength(50)]
        [Display(Name = "Country")]
        public string Country { get; set; } = "Scotland";

        [MaxLength(20)]
        [Display(Name = "Phone number")]
        public string? PhoneNumber { get; set; } = "123-456-789";
    }

    public class CartLineVm
    {
        public int CartItemId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal => UnitPrice * Quantity;
    }
}