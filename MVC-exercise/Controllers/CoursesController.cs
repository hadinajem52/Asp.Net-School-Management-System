using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVC_exercise.Models;
using MVC_exercise.Services;
using MVC_exercise.ViewModels.Courses;

namespace MVC_exercise.Controllers;

[Authorize(Roles = "Admin,Viewer")]
public class CoursesController(SchoolDataService schoolData) : Controller
{
    private const int PageSize = 5;

    [HttpGet]
    public async Task<IActionResult> Index(string? searchTerm, int page = 1)
    {
        var viewModel = new CourseListViewModel
        {
            Courses = await schoolData.GetCoursesPageAsync(searchTerm, page, PageSize),
            SearchTerm = searchTerm?.Trim()
        };

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var course = await schoolData.GetCourseAsync(id);

        if (course is null)
        {
            return NotFound();
        }

        return View(new CourseDetailsViewModel
        {
            Course = course,
            EnrolledStudents = await schoolData.GetStudentsForCourseAsync(id)
        });
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public IActionResult Create()
    {
        return View(new Course());
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Course course)
    {
        if (!ModelState.IsValid)
        {
            return View(course);
        }

        if (!await schoolData.AddCourseAsync(course))
        {
            ModelState.AddModelError(nameof(Course.Name), "A course with this name already exists.");
            return View(course);
        }

        TempData["SuccessMessage"] = "The course was added successfully.";
        return RedirectToAction(nameof(Details), new { id = course.Id });
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var course = await schoolData.GetCourseAsync(id);
        return course is null ? NotFound() : View(course);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Course course)
    {
        if (id != course.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(course);
        }

        if (await schoolData.GetCourseAsync(id) is null)
        {
            return NotFound();
        }

        if (!await schoolData.UpdateCourseAsync(course))
        {
            ModelState.AddModelError(nameof(Course.Name), "A course with this name already exists.");
            return View(course);
        }

        TempData["SuccessMessage"] = "The course was updated successfully.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var course = await schoolData.GetCourseAsync(id);
        return course is null ? NotFound() : View(course);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (!await schoolData.DeleteCourseAsync(id))
        {
            return NotFound();
        }

        TempData["SuccessMessage"] = "The course was deleted successfully.";
        return RedirectToAction(nameof(Index));
    }
}
