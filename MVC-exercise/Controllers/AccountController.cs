using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MVC_exercise.Models;
using MVC_exercise.ViewModels.Account;

namespace MVC_exercise.Controllers;

public class AccountController(
    SignInManager<ApplicationUser> signInManager,
    UserManager<ApplicationUser> userManager) : Controller
{
    [AllowAnonymous]
    [HttpGet]
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        return View(new RegisterViewModel());
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            ApprovalStatus = AccountApprovalStatus.Pending
        };

        var result = await userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
        {
            AddIdentityErrors(result);
            return View(model);
        }

        TempData["RegistrationMessage"] =
            "Your access request was sent. Sign in later to check whether an administrator has approved it.";

        return RedirectToAction(nameof(Login));
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await userManager.FindByEmailAsync(model.Email);
        var passwordIsValid = user is not null &&
            (await signInManager.CheckPasswordSignInAsync(user, model.Password, false)).Succeeded;

        if (!passwordIsValid)
        {
            ModelState.AddModelError(string.Empty, "The email or password is incorrect.");
            return View(model);
        }

        if (user!.ApprovalStatus != AccountApprovalStatus.Approved)
        {
            AddApprovalError(user.ApprovalStatus);
            return View(model);
        }

        await signInManager.SignInAsync(user, model.RememberMe);

        return Url.IsLocalUrl(model.ReturnUrl)
            ? LocalRedirect(model.ReturnUrl)
            : RedirectToAction(nameof(HomeController.Index), "Home");
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await signInManager.SignOutAsync();
        return RedirectToAction(nameof(Login));
    }

    private void AddApprovalError(AccountApprovalStatus approvalStatus)
    {
        var message = approvalStatus == AccountApprovalStatus.Pending
            ? "Your registration is pending administrator approval. Please try again later."
            : "Your registration was not approved. Contact an administrator for help.";

        ModelState.AddModelError(string.Empty, message);
    }

    private void AddIdentityErrors(IdentityResult result)
    {
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
    }
}
