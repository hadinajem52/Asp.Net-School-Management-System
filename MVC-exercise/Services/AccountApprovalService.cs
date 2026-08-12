using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MVC_exercise.Data;
using MVC_exercise.Models;

namespace MVC_exercise.Services;

public class AccountApprovalService(
    UserManager<ApplicationUser> userManager,
    SchoolDbContext db)
{
    public Task<List<ApplicationUser>> GetPendingAccountsAsync()
    {
        return userManager.Users
            .AsNoTracking()
            .Where(user => user.ApprovalStatus == AccountApprovalStatus.Pending)
            .OrderBy(user => user.Email)
            .ToListAsync();
    }

    public async Task<bool> ApproveAsync(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return false;
        }

        await using var transaction = await db.Database.BeginTransactionAsync();
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

        if (!updateResult.Succeeded)
        {
            return false;
        }

        await transaction.CommitAsync();
        return true;
    }

    public async Task<bool> RejectAsync(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return false;
        }

        var user = await userManager.FindByIdAsync(userId);

        if (user is null || user.ApprovalStatus != AccountApprovalStatus.Pending)
        {
            return false;
        }

        user.ApprovalStatus = AccountApprovalStatus.Rejected;
        return (await userManager.UpdateAsync(user)).Succeeded;
    }
}
