using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using razor_exercise.Models;
using razor_exercise.Services;

namespace razor_exercise.Pages.Students;

[Authorize(Policy = "AdminOnly")]
public class DeleteModel(SchoolDataService schoolData) : PageModel {

    public Student Student { get; private set; } = null!;

    public async Task<IActionResult> OnGetAsync(int id){
        
        Student? student = await schoolData.GetStudentAsync(id);

        if (student is null)
        {
            return NotFound();
        }

        Student = student;
        
        return Page();
    } 

    public async Task<IActionResult> OnPostAsync(int id)
    {
        bool deleteSuccessful = await schoolData.DeleteStudentAsync(id);

        if (!deleteSuccessful)
        {
            return NotFound();
        }

        TempData["SuccessMessage"] = "Student was deleted from the directory.";

        return RedirectToPage("./Index");
    }

}
