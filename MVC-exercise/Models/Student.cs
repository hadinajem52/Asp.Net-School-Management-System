using System.ComponentModel.DataAnnotations;

namespace MVC_exercise.Models;

public class Student : IValidatableObject
{
    public int Id { get; set; }

    [Required(ErrorMessage = "First name is required.")]
    [StringLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required.")]
    [StringLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Date of birth is required.")]
    [DataType(DataType.Date)]
    public DateTime? DateOfBirth { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (DateOfBirth is DateTime dateOfBirth && dateOfBirth.Date > DateTime.Today)
        {
            yield return new ValidationResult(
                "Date of birth cannot be in the future.",
                [nameof(DateOfBirth)]);
        }
    }
}
