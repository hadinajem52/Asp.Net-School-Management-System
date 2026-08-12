using MVC_exercise.Models;

namespace MVC_exercise.ViewModels.Students;

public class StudentDetailsViewModel
{
    public required Student Student { get; init; }

    public List<Course> EnrolledCourses { get; init; } = [];
}
