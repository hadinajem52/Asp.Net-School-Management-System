using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using razor_exercise.Models;
using razor_exercise.Services;

namespace razor_exercise.Pages.Students;

[Authorize(Policy = "AdminOnly")]
public class EditModel(SchoolDataService schoolData) : PageModel{

    [BindProperty]
    public Student Student { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Student? student = await schoolData.GetStudentAsync(id);

        if (student is null)
        {
            return NotFound();
        }

        Student = student;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        bool updateSuccessful = await schoolData.UpdateStudentAsync(Student);

        if (!updateSuccessful)
        {
            return NotFound();
        }

        TempData["SuccessMessage"] = $"{Student.FirstName} {Student.LastName} was updated in the directory.";

        return RedirectToPage("./Index");
    }


}
