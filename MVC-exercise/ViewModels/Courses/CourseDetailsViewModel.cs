using MVC_exercise.Models;

namespace MVC_exercise.ViewModels.Courses;

public class CourseDetailsViewModel
{
    public required Course Course { get; init; }

    public List<Student> EnrolledStudents { get; init; } = [];
}
