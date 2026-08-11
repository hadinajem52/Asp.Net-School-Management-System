using Microsoft.AspNetCore.Identity;
using MVC_exercise.Models;

namespace MVC_exercise.Data;

public static class IdentityDataSeeder
{
    private static readonly string[] RoleNames = ["Admin", "Viewer"];

    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();

        foreach (var roleName in RoleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                EnsureSucceeded(
                    await roleManager.CreateAsync(new IdentityRole(roleName)),
                    $"create the {roleName} role");
            }
        }

        var adminEmail = configuration["IdentitySeed:AdminEmail"];
        var adminPassword = configuration["IdentitySeed:AdminPassword"];

        if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
        {
            throw new InvalidOperationException("The administrator seed credentials are missing.");
        }

        var administrator = await userManager.FindByEmailAsync(adminEmail);

        if (administrator is null)
        {
            administrator = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                ApprovalStatus = AccountApprovalStatus.Approved
            };

            EnsureSucceeded(
                await userManager.CreateAsync(administrator, adminPassword),
                "create the initial administrator account");
        }

        if (!await userManager.IsInRoleAsync(administrator, "Admin"))
        {
            EnsureSucceeded(
                await userManager.AddToRoleAsync(administrator, "Admin"),
                "assign the Admin role to the initial administrator account");
        }
    }

    private static void EnsureSucceeded(IdentityResult result, string action)
    {
        if (result.Succeeded)
        {
            return;
        }

        var errors = string.Join(" ", result.Errors.Select(error => error.Description));
        throw new InvalidOperationException($"Unable to {action}. {errors}");
    }
}
