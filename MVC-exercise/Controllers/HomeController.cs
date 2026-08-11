using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVC_exercise.Models;
using MVC_exercise.Services;
using MVC_exercise.ViewModels;

namespace MVC_exercise.Controllers;

[Authorize]
public class HomeController(SchoolDataService schoolData) : Controller
{
    public async Task<IActionResult> Index()
    {
        var viewModel = new DashboardViewModel
        {
            StudentCount = await schoolData.GetStudentCountAsync(),
            CourseCount = await schoolData.GetCourseCountAsync(),
            RecentStudents = await schoolData.GetMostRecentStudentsAsync()
        };

        return View(viewModel);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [AllowAnonymous]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
