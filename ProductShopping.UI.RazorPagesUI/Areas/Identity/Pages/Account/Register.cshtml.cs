#nullable disable

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProductShopping.Application.DTOs.Auth;
using ProductShopping.UI.RazorPagesUI.Contracts;
using System.ComponentModel.DataAnnotations;

namespace ProductShopping.UI.RazorPagesUI.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly IAuthApiClient _authApiClient;
        private readonly ILogger<RegisterModel> _logger;

        public RegisterModel(
            IAuthApiClient authApiClient,
            ILogger<RegisterModel> logger)
        {
            _authApiClient = authApiClient;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "First name")]
            public string FirstName { get; set; }

            [Required]
            [Display(Name = "Last name")]
            public string LastName { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            return Task.CompletedTask;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var registerUserDto = new RegisterUserDto
            {
                Email = Input.Email,
                Password = Input.Password,
                FirstName = Input.FirstName,
                LastName = Input.LastName
            };

            try
            {
                var registeredUser = await _authApiClient.Register(registerUserDto);

                if (registeredUser is null)
                {
                    ModelState.AddModelError(string.Empty, "Registration failed.");
                    return Page();
                }

                _logger.LogInformation("User created a new account through API.");

                return RedirectToPage("RegisterConfirmation", new
                {
                    email = Input.Email,
                    returnUrl = returnUrl
                });
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(string.Empty, "Registration service is currently unavailable.");
                return Page();
            }
        }
    }
}