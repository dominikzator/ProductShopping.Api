namespace ProductShopping.Application.Models.Identity;

public sealed class AuthenticatedUser
{
    public string Id { get; init; } = default!;
    public string Email { get; init; } = default!;
    public string FullName { get; init; } = default!;
    public string UserName { get; init; } = default!;
}
