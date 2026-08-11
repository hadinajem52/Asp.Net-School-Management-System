using Microsoft.AspNetCore.Identity;

namespace razor_exercise.Models;

// This represents a person who can sign in to the application.
// It inherits Identity's built-in user fields, such as Id, UserName, and Email.
public class ApplicationUser : IdentityUser
{


    // New sign-ups begin as Pending. An administrator will decide whether to approve them.
    public AccountApprovalStatus ApprovalStatus { get; set; } = AccountApprovalStatus.Pending;



}
