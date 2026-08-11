using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using razor_exercise.Models;
using razor_exercise.Services;

namespace razor_exercise.Pages.Admin;

[Authorize(Policy = "AdminOnly")]
public class AccountRequestsModel(AccountApprovalService accountApproval) : PageModel
{
    public List<ApplicationUser> PendingAccounts { get; private set; } = [];

    public async Task OnGetAsync()
    {
        PendingAccounts = await accountApproval.GetPendingAccountsAsync();
    }

    public async Task<IActionResult> OnPostApproveAsync(string id)
    {
        if (!await accountApproval.ApproveAsync(id))
        {
            TempData["ErrorMessage"] = "That account request could not be approved. Refresh the page and try again.";
        }
        else
        {
            TempData["SuccessMessage"] = "The account was approved and can now sign in as a Viewer.";
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostRejectAsync(string id)
    {
        if (!await accountApproval.RejectAsync(id))
        {
            TempData["ErrorMessage"] = "That account request could not be rejected. Refresh the page and try again.";
        }
        else
        {
            TempData["SuccessMessage"] = "The account request was rejected.";
        }

        return RedirectToPage();
    }
}
