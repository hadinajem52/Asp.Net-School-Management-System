using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using razor_exercise.Models;
using razor_exercise.Services;

namespace razor_exercise.Pages.Courses;

[Authorize(Policy = "AdminOnly")]
public class CreateModel(SchoolDataService schoolData) : PageModel
{
    [BindProperty]
    public Course Course { get; set; } = new();

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (!await schoolData.AddCourseAsync(Course))
        {
            ModelState.AddModelError("Course.Name", "A course with this name already exists.");
            return Page();
        }

        TempData["SuccessMessage"] = $"{Course.Name} was added to the catalogue.";
        return RedirectToPage("./Index");
    }
}
