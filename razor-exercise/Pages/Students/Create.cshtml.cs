using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using razor_exercise.Models;
using razor_exercise.Services;

namespace razor_exercise.Pages.Students;

[Authorize(Policy = "AdminOnly")]
public class CreateModel(SchoolDataService schoolData) : PageModel
{

    // when the form is submitted, take the submitted data and bind it to the Student object.
    [BindProperty]
    public Student Student { get; set; } = new();

    public void OnGet()
    {
    }

    // This handler finishes later because it awaits work, and then it returns an HTTP action/result
    // IActionResult is an interface that represents the result of an action method.
    public async Task<IActionResult> OnPostAsync()
    {
        //show the current form again
        if (!ModelState.IsValid)
        {
            return Page();
        }

        
        await schoolData.AddStudentAsync(Student);

        TempData["SuccessMessage"] = $"{Student.FirstName} {Student.LastName} was added to the directory.";

        return RedirectToPage("./Index");

    }
}
