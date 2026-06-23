// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProductShopping.Application.DTOs.Auth;
using ProductShopping.UI.Shared.Contracts;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ProductShopping.UI.RazorPagesUI.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly IAuthApiClient _authApiClient;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(IAuthApiClient authApiClient, ILogger<LoginModel> logger)
        {
            _authApiClient = authApiClient;
            _logger = logger;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string ErrorMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var loginUserDto = new LoginUserDto
            {
                Email = Input.Email,
                Password = Input.Password
            };

            string rawResponse;

            try
            {
                rawResponse = await _authApiClient.LoginAsync(loginUserDto);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Login service is unavailable for {Email}", Input.Email);
                ModelState.AddModelError(string.Empty, "Login service is currently unavailable.");
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during login for {Email}", Input.Email);
                ModelState.AddModelError(string.Empty, "An unexpected error occurred during login.");
                return Page();
            }

            if (string.IsNullOrWhiteSpace(rawResponse))
            {
                _logger.LogWarning("Login returned empty response for {Email}", Input.Email);
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return Page();
            }

            _logger.LogInformation("Raw login response: {RawResponse}", rawResponse);

            var jwtToken = rawResponse.Trim();

            if (jwtToken.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                jwtToken = jwtToken["Bearer ".Length..].Trim();
            }

            var handler = new JwtSecurityTokenHandler();

            if (!handler.CanReadToken(jwtToken))
            {
                _logger.LogWarning("Login returned non-JWT response for {Email}. Response: {RawResponse}", Input.Email, rawResponse);
                ModelState.AddModelError(string.Empty, "Login failed. API did not return a valid token.");
                return Page();
            }

            JwtSecurityToken jwt;

            try
            {
                jwt = handler.ReadJwtToken(jwtToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to parse JWT for {Email}. Token: {Token}", Input.Email, jwtToken);
                ModelState.AddModelError(string.Empty, "Login failed. Invalid token format.");
                return Page();
            }

            var claims = new List<Claim>();

            foreach (var claim in jwt.Claims)
            {
                var mappedType = claim.Type switch
                {
                    "email" => ClaimTypes.Email,
                    "unique_name" => ClaimTypes.Name,
                    "role" => ClaimTypes.Role,
                    _ => claim.Type
                };

                claims.Add(new Claim(mappedType, claim.Value));
            }

            claims.Add(new Claim("access_token", jwtToken));

            if (!claims.Any(c => c.Type == ClaimTypes.Name))
            {
                var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value ?? Input.Email;
                claims.Add(new Claim(ClaimTypes.Name, email));
            }

            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = Input.RememberMe,
                ExpiresUtc = Input.RememberMe
                    ? DateTimeOffset.UtcNow.AddDays(7)
                    : null
            };

            var claimsIdentity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);

            await HttpContext.SignInAsync(
                IdentityConstants.ApplicationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            _logger.LogInformation("User logged in through API for {Email}", Input.Email);

            return LocalRedirect(returnUrl);
        }
    }
}
