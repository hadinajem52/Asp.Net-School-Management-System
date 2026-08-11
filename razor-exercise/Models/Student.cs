using System.ComponentModel.DataAnnotations;

namespace razor_exercise.Models;


// we use IValidatableObject to implement custom validation logic 
// for the Student model. This allows us to define validation rules 
// that go beyond the built-in validation attributes

public class Student : IValidatableObject
{
    public int Id { get; set; }

    //these are validation attributes that will be used to validate the model properties
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

    // we needed this validation method because the built-in validation attributes do not cover all scenarios,
    // such as checking if the date of birth is in the future. By implementing IValidatableObject, 
    // we can create custom validation logic that runs when the model is validated. 

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (DateOfBirth is DateTime dateOfBirth && dateOfBirth.Date > DateTime.Today)
        {
            yield return new ValidationResult("Date of birth cannot be in the future.", [nameof(DateOfBirth)]);
        }
    }
}
