using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using razor_exercise.Models;
using razor_exercise.Services;

namespace razor_exercise.Pages.Courses;

public class DetailsModel(SchoolDataService schoolData) : PageModel
{
    public Course Course { get; private set; } = null!;

    public List<Student> EnrolledStudents { get; private set; } = [];

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Course? course = await schoolData.GetCourseAsync(id);

        if (course is null)
        {
            return NotFound();
        }

        Course = course;
        EnrolledStudents = await schoolData.GetStudentsForCourseAsync(Course.Id);
        return Page();
    }
}
