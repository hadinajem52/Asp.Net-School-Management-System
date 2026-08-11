using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using razor_exercise.Models;
using razor_exercise.Services;

namespace razor_exercise.Pages.Students;

public class DetailsModel(
    SchoolDataService schoolData,
    IAuthorizationService authorizationService) : PageModel
{
    // set is private because we don't want to allow the view to change the Student property, 
    // only read it.
    public Student Student { get; private set; } = null!;

    public List<Course> EnrolledCourses { get; private set; } = [];

    public async Task<IActionResult> OnGetAsync(int id)
    {

        Student? student = await schoolData.GetStudentAsync(id);

        if (student is null)
        {
            return NotFound();
        }

        Student = student;
        EnrolledCourses = await schoolData.GetCoursesForStudentAsync(Student.Id);
        
        return Page();
    }

    public async Task<IActionResult> OnPostRemoveEnrollmentAsync(int id, int courseId)
    {
        var authorizationResult = await authorizationService.AuthorizeAsync(User, "AdminOnly");

        if (!authorizationResult.Succeeded)
        {
            return Forbid();
        }

        Course? course = await schoolData.GetCourseAsync(courseId);

        if (!await schoolData.RemoveEnrollmentAsync(id, courseId))
        {
            return NotFound();
        }

        TempData["SuccessMessage"] = $"{course?.Name ?? "The course"} was removed from this student's schedule.";
        return RedirectToPage("./Details", new { id });
    }
}
