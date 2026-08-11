using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using razor_exercise.Models;
using razor_exercise.Services;

namespace razor_exercise.Pages.Courses;

[Authorize(Policy = "AdminOnly")]
public class EditModel(SchoolDataService schoolData) : PageModel
{
    [BindProperty]
    public Course Course { get; set; } = null!;

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

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (await schoolData.UpdateCourseAsync(Course))
        {
            TempData["SuccessMessage"] = $"{Course.Name} was updated in the catalogue.";
            return RedirectToPage("./Index");
        }

        if (await schoolData.GetCourseAsync(Course.Id) is null)
        {
            return NotFound();
        }

        ModelState.AddModelError("Course.Name", "A course with this name already exists.");
        return Page();
    }
}
