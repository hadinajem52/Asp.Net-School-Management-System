using Microsoft.AspNetCore.Identity;
using razor_exercise.Models;

namespace razor_exercise.Data;

public static class IdentityDataSeeder
{
    private static readonly string[] RoleNames = ["Admin", "Viewer"];

    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        var logger = serviceProvider.GetRequiredService<ILoggerFactory>()
            .CreateLogger("IdentityDataSeeder");

        foreach (var roleName in RoleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                EnsureSucceeded(roleResult, $"create the {roleName} role");
                logger.LogInformation("Created the {RoleName} role.", roleName);
            }
        }

        var adminEmail = configuration["IdentitySeed:AdminEmail"];
        var adminPassword = configuration["IdentitySeed:AdminPassword"];

        if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
        {
            logger.LogWarning(
                "The initial administrator was not created because IdentitySeed:AdminEmail or IdentitySeed:AdminPassword is missing.");
            return;
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

            var userResult = await userManager.CreateAsync(administrator, adminPassword);
            EnsureSucceeded(userResult, "create the initial administrator account");
            logger.LogInformation("Created the initial administrator account.");
        }

        if (!await userManager.IsInRoleAsync(administrator, "Admin"))
        {
            var roleResult = await userManager.AddToRoleAsync(administrator, "Admin");
            EnsureSucceeded(roleResult, "assign the Admin role to the initial administrator account");
            logger.LogInformation("Assigned the Admin role to the initial administrator account.");
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
