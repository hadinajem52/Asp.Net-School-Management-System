using MVC_exercise.Models;

namespace MVC_exercise.ViewModels.Students;

public class StudentListViewModel
{
    public PagedResult<Student> Students { get; init; } = new();

    public string? SearchTerm { get; init; }

    public string? SortBy { get; init; }
}
