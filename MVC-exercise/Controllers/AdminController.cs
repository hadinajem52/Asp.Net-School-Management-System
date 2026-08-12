using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVC_exercise.Services;

namespace MVC_exercise.Controllers;



[Authorize(Roles = "Admin")]
[Route("Admin/AccountRequests")]
public class AdminController(AccountApprovalService accountApproval) : Controller
{

    //we use route attribute because the controller name is not AdminController but Admin/AccountRequests
    [HttpGet("")]
    public async Task<IActionResult> AccountRequests()
    {
        return View(await accountApproval.GetPendingAccountsAsync());
    }

    [HttpPost("Approve")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(string id)
    {
        var wasApproved = await accountApproval.ApproveAsync(id);
        TempData[wasApproved ? "SuccessMessage" : "ErrorMessage"] = wasApproved
            ? "The account was approved and can now sign in as a Viewer."
            : "That account request could not be approved. Refresh the page and try again.";

        return RedirectToAction(nameof(AccountRequests));
    }

    [HttpPost("Reject")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(string id)
    {
        var wasRejected = await accountApproval.RejectAsync(id);
        TempData[wasRejected ? "SuccessMessage" : "ErrorMessage"] = wasRejected
            ? "The account request was rejected."
            : "That account request could not be rejected. Refresh the page and try again.";

        return RedirectToAction(nameof(AccountRequests));
    }
}
