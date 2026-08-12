using MVC_exercise.Models;

namespace MVC_exercise.ViewModels.Courses;

public class CourseListViewModel
{
    public PagedResult<Course> Courses { get; init; } = new();

    public string? SearchTerm { get; init; }
}
