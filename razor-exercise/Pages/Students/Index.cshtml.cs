using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using razor_exercise.Components;
using razor_exercise.Models;
using razor_exercise.Services;

namespace razor_exercise.Pages.Students;

public class IndexModel(SchoolDataService schoolData) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? SortBy { get; set; }

    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1;

    public List<Student> Students { get; private set; } = [];

    public int TotalStudentCount { get; private set; }

    public int TotalPages { get; private set; }

    public IReadOnlyList<SortOption> SortOptions { get; } =
    [
        new("", "Default: last name"),
        new("FirstName", "First name"),
        new("LastName", "Last name"),
        new("DateOfBirth", "Date of birth")
    ];

    public string? ClearSearchUrl => string.IsNullOrWhiteSpace(SearchTerm)
        ? null
        : string.IsNullOrWhiteSpace(SortBy)
            ? "/Students"
            : $"/Students?SortBy={Uri.EscapeDataString(SortBy)}";

    public string EmptyMessage => string.IsNullOrWhiteSpace(SearchTerm)
        ? "Add the first student record to begin building the directory."
        : $"No student names match “{SearchTerm}”.";

    public async Task OnGetAsync()
    {
        PagedResult<Student> result = await schoolData.GetStudentsPageAsync(
            SearchTerm,
            SortBy,
            PageNumber,
            pageSize: 5);

        Students = result.Items;
        PageNumber = result.PageNumber;
        TotalPages = result.TotalPages;
        TotalStudentCount = result.TotalCount;
    }
}
