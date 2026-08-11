using MVC_exercise.Models;

namespace MVC_exercise.ViewModels;

public class DashboardViewModel
{
    public int StudentCount { get; init; }

    public int CourseCount { get; init; }

    public List<Student> RecentStudents { get; init; } = [];
}
