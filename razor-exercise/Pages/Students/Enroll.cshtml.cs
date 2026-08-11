using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using razor_exercise.Models;
using razor_exercise.Services;

namespace razor_exercise.Pages.Students;

[Authorize(Policy = "AdminOnly")]
public class EnrollModel(SchoolDataService schoolData) : PageModel
{
    public Student Student { get; private set; } = null!;

    public List<Course> AvailableCourses { get; private set; } = [];

    [BindProperty]
    public List<int> SelectedCourseIds { get; set; } = [];

    public async Task<IActionResult> OnGetAsync(int id)
    {
        if (!await LoadPageDataAsync(id))
        {
            return NotFound();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        if (!await LoadPageDataAsync(id))
        {
            return NotFound();
        }

        if (SelectedCourseIds.Count == 0)
        {
            ModelState.AddModelError(string.Empty, "Select at least one course.");
            return Page();
        }

        if (!await schoolData.EnrollStudentInCoursesAsync(id, SelectedCourseIds))
        {
            ModelState.AddModelError(string.Empty, "The selected courses could not be added. Refresh the page and try again.");
            return Page();
        }

        int enrolledCourseCount = SelectedCourseIds.Distinct().Count();
        string courseLabel = enrolledCourseCount == 1 ? "course was" : "courses were";
        TempData["SuccessMessage"] = $"{enrolledCourseCount} {courseLabel} added for {Student.FirstName} {Student.LastName}.";

        return RedirectToPage("./Details", new { id });
        
    }

    private async Task<bool> LoadPageDataAsync(int studentId)
    {
        Student? student = await schoolData.GetStudentAsync(studentId);

        if (student is null)
        {
            return false;
        }

        Student = student;

        AvailableCourses = await schoolData.GetAvailableCoursesForStudentAsync(studentId);

        return true;
    }
}
