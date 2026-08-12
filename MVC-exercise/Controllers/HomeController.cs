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
    // we dont use route attribute because the controller name is HomeController and the action name is Index,
    // so the route will be /Home/Index
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

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [AllowAnonymous]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
