using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using razor_exercise.Models;
using razor_exercise.Services;

namespace razor_exercise.Pages.Courses;

[Authorize(Policy = "AdminOnly")]
public class DeleteModel(SchoolDataService schoolData) : PageModel
{
    public Course Course { get; private set; } = null!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Course? course = await schoolData.GetCourseAsync(id);

        if (course is null)
        {
            return NotFound();
        }

        Course = course;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        if (!await schoolData.DeleteCourseAsync(id))
        {
            return NotFound();
        }

        TempData["SuccessMessage"] = "Course was deleted from the catalogue.";
        return RedirectToPage("./Index");
    }
}
