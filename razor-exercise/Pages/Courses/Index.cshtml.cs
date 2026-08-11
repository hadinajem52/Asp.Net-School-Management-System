using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using razor_exercise.Models;
using razor_exercise.Services;

namespace razor_exercise.Pages.Courses;

public class IndexModel(SchoolDataService schoolData) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1;

    public List<Course> Courses { get; private set; } = [];

    public int TotalCourseCount { get; private set; }

    public int TotalPages { get; private set; }

    public string? ClearSearchUrl => string.IsNullOrWhiteSpace(SearchTerm) ? null : "/Courses";

    public string EmptyMessage => string.IsNullOrWhiteSpace(SearchTerm)
        ? "Add the first course to begin building the catalogue."
        : $"No course names match “{SearchTerm}”.";

    public async Task OnGetAsync()
    {
        PagedResult<Course> result = await schoolData.GetCoursesPageAsync(
            SearchTerm,
            PageNumber,
            pageSize: 6);

        Courses = result.Items;
        PageNumber = result.PageNumber;
        TotalPages = result.TotalPages;
        TotalCourseCount = result.TotalCount;
    }
}
