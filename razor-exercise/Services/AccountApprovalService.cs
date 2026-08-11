using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using razor_exercise.Models;

namespace razor_exercise.Services;

public class AccountApprovalService(UserManager<ApplicationUser> userManager)
{
    public async Task<List<ApplicationUser>> GetPendingAccountsAsync()
    {
        return await userManager.Users
            .Where(user => user.ApprovalStatus == AccountApprovalStatus.Pending)
            .OrderBy(user => user.Email)
            .ToListAsync();
    }

    public async Task<bool> ApproveAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);

        if (user is null || user.ApprovalStatus != AccountApprovalStatus.Pending)
        {
            return false;
        }

        if (!await userManager.IsInRoleAsync(user, "Viewer"))
        {
            var roleResult = await userManager.AddToRoleAsync(user, "Viewer");

            if (!roleResult.Succeeded)
            {
                return false;
            }
        }

        user.ApprovalStatus = AccountApprovalStatus.Approved;
        var updateResult = await userManager.UpdateAsync(user);
        return updateResult.Succeeded;
    }

    public async Task<bool> RejectAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);

        if (user is null || user.ApprovalStatus != AccountApprovalStatus.Pending)
        {
            return false;
        }

        user.ApprovalStatus = AccountApprovalStatus.Rejected;
        var updateResult = await userManager.UpdateAsync(user);
        return updateResult.Succeeded;
    }
}
