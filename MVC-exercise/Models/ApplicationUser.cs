using Microsoft.AspNetCore.Identity;

namespace MVC_exercise.Models;

public class ApplicationUser : IdentityUser
{
    public AccountApprovalStatus ApprovalStatus { get; set; } = AccountApprovalStatus.Pending;
}
