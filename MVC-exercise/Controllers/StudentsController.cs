using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVC_exercise.Models;
using MVC_exercise.Services;
using MVC_exercise.ViewModels.Students;

namespace MVC_exercise.Controllers;

[Authorize(Roles = "Admin,Viewer")]
public class StudentsController(SchoolDataService schoolData) : Controller
{
    private const int PageSize = 5;

    [HttpGet]
    public async Task<IActionResult> Index(
        string? searchTerm,
        string? sortBy,
        int page = 1)
    {
        var viewModel = new StudentListViewModel
        {
            Students = await schoolData.GetStudentsPageAsync(searchTerm, sortBy, page, PageSize),
            SearchTerm = searchTerm?.Trim(),
            SortBy = sortBy
        };

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var student = await schoolData.GetStudentAsync(id);

        if (student is null)
        {
            return NotFound();
        }

        return View(new StudentDetailsViewModel
        {
            Student = student,
            EnrolledCourses = await schoolData.GetCoursesForStudentAsync(id)
        });
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public IActionResult Create()
    {
        return View(new Student());
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Student student)
    {
        if (!ModelState.IsValid)
        {
            return View(student);
        }

        await schoolData.AddStudentAsync(student);
        TempData["SuccessMessage"] = "The student was added successfully.";
        return RedirectToAction(nameof(Details), new { id = student.Id });
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var student = await schoolData.GetStudentAsync(id);
        return student is null ? NotFound() : View(student);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Student student)
    {
        if (id != student.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(student);
        }

        if (!await schoolData.UpdateStudentAsync(student))
        {
            return NotFound();
        }

        TempData["SuccessMessage"] = "The student was updated successfully.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var student = await schoolData.GetStudentAsync(id);
        return student is null ? NotFound() : View(student);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (!await schoolData.DeleteStudentAsync(id))
        {
            return NotFound();
        }

        TempData["SuccessMessage"] = "The student was deleted successfully.";
        return RedirectToAction(nameof(Index));
    }
}
