using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProductShopping.Application.Features.CartItem.Commands.AddCartItem;
using ProductShopping.Application.Features.CartItem.Commands.RemoveCartItem;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using ProductShopping.Application.Models.Paging;
using ProductShopping.UI.RazorPagesUI.Contracts;

namespace ProductShopping.UI.RazorPagesUI.Pages.Cart;

public class CartModel(ICartsApiClient cartsApiClient) : PageModel
{
    public List<CartLineVm> Items { get; private set; } = [];
    public int TotalQuantity => Items.Sum(x => x.Quantity);
    public decimal Subtotal => Items.Sum(x => x.LineTotal);

    [TempData]
    public string? StatusMessage { get; set; }

    [TempData]
    public bool IsError { get; set; }

    public async Task<IActionResult> OnGetAsync(CancellationToken ct)
    {
        if (!User.Identity?.IsAuthenticated ?? true)
        {
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        }

        await LoadCartAsync(ct);

        return Page();
    }

    private bool IsAjaxRequest()
    => string.Equals(Request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.OrdinalIgnoreCase);

    public async Task<IActionResult> OnGetContentAsync(CancellationToken ct)
    {
        if (!User.Identity?.IsAuthenticated ?? true)
        {
            return new ContentResult
            {
                StatusCode = 401,
                Content = string.Empty
            };
        }

        await LoadCartAsync(ct);

        return new PartialViewResult
        {
            ViewName = "_CartContent",
            ViewData = new ViewDataDictionary<CartModel>(ViewData, this)
        };
    }

    public async Task<IActionResult> OnPostIncreaseQuantityAsync(int cartItemId, CancellationToken ct)
    {
        if (!await TryLoadCartOrRedirectAsync(ct))
        {
            if (IsAjaxRequest())
            {
                return new JsonResult(new { success = false, message = "You need to sign in first." })
                {
                    StatusCode = 401
                };
            }

            return RedirectToPage();
        }

        var item = Items.FirstOrDefault(x => x.CartItemId == cartItemId);
        if (item is null)
        {
            if (IsAjaxRequest())
            {
                return new JsonResult(new { success = false, message = "Cart item not found." })
                {
                    StatusCode = 404
                };
            }

            SetError("Cart item not found.");
            return RedirectToPage();
        }

        if (item.Quantity >= item.MaxQuantity)
        {
            if (IsAjaxRequest())
            {
                return new JsonResult(new { success = false, message = "You cannot add more items than are currently available." })
                {
                    StatusCode = 400
                };
            }

            SetError("You cannot add more items than are currently available.");
            return RedirectToPage();
        }

        var token = GetAccessToken();
        await cartsApiClient.AddCartItemAsync(token!, new AddCartItemCommand
        {
            ProductId = item.ProductId,
            Quantity = 1
        }, ct);

        await LoadCartAsync(ct);

        if (IsAjaxRequest())
        {
            return new JsonResult(new
            {
                success = true,
                message = "",
                cartItemsCount = TotalQuantity
            });
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDecreaseQuantityAsync(int cartItemId, CancellationToken ct)
    {
        if (!await TryLoadCartOrRedirectAsync(ct))
        {
            if (IsAjaxRequest())
            {
                return new JsonResult(new { success = false, message = "You need to sign in first." })
                {
                    StatusCode = 401
                };
            }

            return RedirectToPage();
        }

        var item = Items.FirstOrDefault(x => x.CartItemId == cartItemId);
        if (item is null)
        {
            if (IsAjaxRequest())
            {
                return new JsonResult(new { success = false, message = "Cart item not found." })
                {
                    StatusCode = 404
                };
            }

            SetError("Cart item not found.");
            return RedirectToPage();
        }

        var token = GetAccessToken();

        await cartsApiClient.RemoveCartItemAsync(token!, new RemoveCartItemCommand
        {
            CartItemId = cartItemId,
            Quantity = 1
        }, ct);

        await LoadCartAsync(ct);

        if (IsAjaxRequest())
        {
            return new JsonResult(new
            {
                success = true,
                message = "",
                cartItemsCount = TotalQuantity
            });
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostRemoveItemAsync(int cartItemId, CancellationToken ct)
    {
        var token = GetAccessToken();
        if (string.IsNullOrWhiteSpace(token))
        {
            if (IsAjaxRequest())
            {
                return new JsonResult(new { success = false, message = "You need to sign in first." })
                {
                    StatusCode = 401
                };
            }

            SetError("You need to sign in first.");
            return RedirectToPage();
        }

        var result = await cartsApiClient.GetCartItemsAsync(token, new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 100
        }, ct);

        var item = result.Data.FirstOrDefault(x => x.Id == cartItemId);
        if (item is null)
        {
            if (IsAjaxRequest())
            {
                return new JsonResult(new { success = false, message = "Cart item not found." })
                {
                    StatusCode = 404
                };
            }

            SetError("Cart item not found.");
            return RedirectToPage();
        }

        await cartsApiClient.RemoveCartItemAsync(token, new RemoveCartItemCommand
        {
            CartItemId = cartItemId,
            Quantity = item.Quantity
        }, ct);

        await LoadCartAsync(ct);

        if (IsAjaxRequest())
        {
            return new JsonResult(new
            {
                success = true,
                message = "Item removed from cart.",
                cartItemsCount = TotalQuantity
            });
        }

        return RedirectToPage();
    }

    public IActionResult OnPostCheckout()
    {
        return RedirectToPage();
    }

    private async Task<bool> TryLoadCartOrRedirectAsync(CancellationToken ct)
    {
        if (!User.Identity?.IsAuthenticated ?? true)
        {
            SetError("You need to sign in first.");
            return false;
        }

        await LoadCartAsync(ct);
        return true;
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
                UnitPrice = x.UnitPrice,
                MaxQuantity = 999
            })
            .ToList() ?? [];
    }

    private string? GetAccessToken()
        => User.FindFirst("access_token")?.Value;

    private void SetSuccess(string message)
    {
        StatusMessage = message;
        IsError = false;
    }

    private void SetError(string message)
    {
        StatusMessage = message;
        IsError = true;
    }

    public class CartLineVm
    {
        public int CartItemId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public int Quantity { get; set; }
        public int MaxQuantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal => UnitPrice * Quantity;
    }
}