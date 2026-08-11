using System.ComponentModel.DataAnnotations;

namespace razor_exercise.Models;

public class Course
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Course name is required.")]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Course description is required.")]
    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    [Range(1, 6, ErrorMessage = "Credits must be between 1 and 6.")]
    public int Credits { get; set; }
}
