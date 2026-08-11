using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using razor_exercise.Models;

namespace razor_exercise.Pages.Account;

[AllowAnonymous]
public class LoginModel(
    SignInManager<ApplicationUser> signInManager,
    UserManager<ApplicationUser> userManager) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? ReturnUrl { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Check the password without creating a cookie. Pending and rejected accounts must not sign in.
        var user = await userManager.FindByEmailAsync(Input.Email);

        if (user is null)
        {
            ModelState.AddModelError(string.Empty, "The email or password is incorrect.");
            return Page();
        }

        var passwordResult = await signInManager.CheckPasswordSignInAsync(
            user,
            Input.Password,
            lockoutOnFailure: false);

        if (!passwordResult.Succeeded)
        {
            ModelState.AddModelError(string.Empty, "The email or password is incorrect.");
            return Page();
        }

        if (user.ApprovalStatus == AccountApprovalStatus.Pending)
        {
            ModelState.AddModelError(string.Empty, "Your registration is pending administrator approval. Please try again later.");
            return Page();
        }

        if (user.ApprovalStatus == AccountApprovalStatus.Rejected)
        {
            ModelState.AddModelError(string.Empty, "Your registration was not approved. Contact an administrator for help.");
            return Page();
        }

        await signInManager.SignInAsync(user, Input.RememberMe);
        return LocalRedirect(ReturnUrl ?? Url.Content("~/"));
    }

    public class InputModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }
}
