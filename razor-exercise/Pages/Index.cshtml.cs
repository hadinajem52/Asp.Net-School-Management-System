using Microsoft.AspNetCore.Mvc.RazorPages;
using razor_exercise.Models;
using razor_exercise.Services;

namespace razor_exercise.Pages;

public class IndexModel(SchoolDataService schoolData) : PageModel
{
    public int StudentCount { get; private set; }

    public int CourseCount { get; private set; }

    public List<Student> RecentStudents { get; private set; } = [];

    public async Task OnGetAsync()
    {
        StudentCount = await schoolData.GetStudentCountAsync();
        CourseCount = await schoolData.GetCourseCountAsync();
        RecentStudents = await schoolData.GetMostRecentStudentsAsync();
    }
    
}
