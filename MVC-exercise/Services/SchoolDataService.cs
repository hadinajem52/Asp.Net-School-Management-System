using Microsoft.EntityFrameworkCore;
using MVC_exercise.Data;
using MVC_exercise.Models;

namespace MVC_exercise.Services;

public class SchoolDataService(SchoolDbContext db)
{
    public Task<int> GetStudentCountAsync()
    {
        return db.Students.CountAsync();
    }

    public Task<int> GetCourseCountAsync()
    {
        return db.Courses.CountAsync();
    }

    public Task<List<Student>> GetMostRecentStudentsAsync()
    {
        return db.Students
            .AsNoTracking()
            .OrderByDescending(student => student.Id)
            .Take(5)
            .ToListAsync();
    }

    public Task<List<Student>> GetStudentsAsync(
        string? searchTerm = null,
        string? sortBy = null)
    {
        return OrderStudents(FilterStudents(searchTerm), sortBy).ToListAsync();
    }

    public Task<PagedResult<Student>> GetStudentsPageAsync(
        string? searchTerm,
        string? sortBy,
        int pageNumber,
        int pageSize)
    {
        var students = OrderStudents(FilterStudents(searchTerm), sortBy);
        return CreatePageAsync(students, pageNumber, pageSize);
    }

    public Task<Student?> GetStudentAsync(int studentId)
    {
        return db.Students
            .AsNoTracking()
            .FirstOrDefaultAsync(student => student.Id == studentId);
    }

    public async Task AddStudentAsync(Student student)
    {
        db.Students.Add(student);
        await db.SaveChangesAsync();
    }

    public async Task<bool> UpdateStudentAsync(Student updatedStudent)
    {
        var student = await db.Students.FindAsync(updatedStudent.Id);

        if (student is null)
        {
            return false;
        }

        student.FirstName = updatedStudent.FirstName;
        student.LastName = updatedStudent.LastName;
        student.Email = updatedStudent.Email;
        student.DateOfBirth = updatedStudent.DateOfBirth;
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteStudentAsync(int studentId)
    {
        var student = await db.Students.FindAsync(studentId);

        if (student is null)
        {
            return false;
        }

        db.Students.Remove(student);
        await db.SaveChangesAsync();
        return true;
    }

    public Task<List<Course>> GetCoursesAsync(string? searchTerm = null)
    {
        return FilterCourses(searchTerm)
            .OrderBy(course => course.Name)
            .ToListAsync();
    }

    public Task<PagedResult<Course>> GetCoursesPageAsync(
        string? searchTerm,
        int pageNumber,
        int pageSize)
    {
        var courses = FilterCourses(searchTerm).OrderBy(course => course.Name);
        return CreatePageAsync(courses, pageNumber, pageSize);
    }

    public Task<Course?> GetCourseAsync(int courseId)
    {
        return db.Courses
            .AsNoTracking()
            .FirstOrDefaultAsync(course => course.Id == courseId);
    }

    public async Task<bool> AddCourseAsync(Course course)
    {
        course.Name = course.Name.Trim();

        if (!await IsCourseNameAvailableAsync(course.Name))
        {
            return false;
        }

        db.Courses.Add(course);
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateCourseAsync(Course updatedCourse)
    {
        var course = await db.Courses.FindAsync(updatedCourse.Id);
        var courseName = updatedCourse.Name.Trim();

        if (course is null || !await IsCourseNameAvailableAsync(courseName, updatedCourse.Id))
        {
            return false;
        }

        course.Name = courseName;
        course.Description = updatedCourse.Description;
        course.Credits = updatedCourse.Credits;
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteCourseAsync(int courseId)
    {
        var course = await db.Courses.FindAsync(courseId);

        if (course is null)
        {
            return false;
        }

        db.Courses.Remove(course);
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> EnrollStudentAsync(int studentId, int courseId)
    {
        return await EnrollStudentInCoursesAsync(studentId, [courseId]);
    }

    public async Task<bool> EnrollStudentInCoursesAsync(
        int studentId,
        IEnumerable<int> courseIds)
    {
        var selectedCourseIds = courseIds.Distinct().ToArray();

        if (selectedCourseIds.Length == 0 || !await StudentExistsAsync(studentId))
        {
            return false;
        }

        var validCourseCount = await db.Courses
            .CountAsync(course => selectedCourseIds.Contains(course.Id));
        var hasExistingEnrollment = await db.Enrollments.AnyAsync(enrollment =>
            enrollment.StudentId == studentId && selectedCourseIds.Contains(enrollment.CourseId));

        if (validCourseCount != selectedCourseIds.Length || hasExistingEnrollment)
        {
            return false;
        }

        db.Enrollments.AddRange(selectedCourseIds.Select(courseId => new Enrollment
        {
            StudentId = studentId,
            CourseId = courseId
        }));

        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveEnrollmentAsync(int studentId, int courseId)
    {
        var enrollment = await db.Enrollments.FindAsync(studentId, courseId);

        if (enrollment is null)
        {
            return false;
        }

        db.Enrollments.Remove(enrollment);
        await db.SaveChangesAsync();
        return true;
    }

    public Task<List<Course>> GetCoursesForStudentAsync(int studentId)
    {
        return db.Courses
            .AsNoTracking()
            .Where(course => db.Enrollments.Any(enrollment =>
                enrollment.StudentId == studentId && enrollment.CourseId == course.Id))
            .OrderBy(course => course.Name)
            .ToListAsync();
    }

    public Task<List<Course>> GetAvailableCoursesForStudentAsync(int studentId)
    {
        return db.Courses
            .AsNoTracking()
            .Where(course => !db.Enrollments.Any(enrollment =>
                enrollment.StudentId == studentId && enrollment.CourseId == course.Id))
            .OrderBy(course => course.Name)
            .ToListAsync();
    }

    public Task<List<Student>> GetStudentsForCourseAsync(int courseId)
    {
        return db.Students
            .AsNoTracking()
            .Where(student => db.Enrollments.Any(enrollment =>
                enrollment.CourseId == courseId && enrollment.StudentId == student.Id))
            .OrderBy(student => student.LastName)
            .ThenBy(student => student.FirstName)
            .ToListAsync();
    }

    private IQueryable<Student> FilterStudents(string? searchTerm)
    {
        var students = db.Students.AsNoTracking();

        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return students;
        }

        var searchPattern = $"%{searchTerm.Trim()}%";
        return students.Where(student =>
            EF.Functions.ILike(student.FirstName, searchPattern) ||
            EF.Functions.ILike(student.LastName, searchPattern) ||
            EF.Functions.ILike(student.FirstName + " " + student.LastName, searchPattern));
    }

    private static IOrderedQueryable<Student> OrderStudents(
        IQueryable<Student> students,
        string? sortBy)
    {
        return sortBy switch
        {
            "FirstName" => students
                .OrderBy(student => student.FirstName)
                .ThenBy(student => student.LastName),
            "LastName" => students
                .OrderBy(student => student.LastName)
                .ThenBy(student => student.FirstName),
            "DateOfBirth" => students
                .OrderBy(student => student.DateOfBirth)
                .ThenBy(student => student.LastName),
            _ => students
                .OrderBy(student => student.LastName)
                .ThenBy(student => student.FirstName)
        };
    }

    private IQueryable<Course> FilterCourses(string? searchTerm)
    {
        var courses = db.Courses.AsNoTracking();

        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return courses;
        }

        var searchPattern = $"%{searchTerm.Trim()}%";
        return courses.Where(course => EF.Functions.ILike(course.Name, searchPattern));
    }

    private Task<bool> StudentExistsAsync(int studentId)
    {
        return db.Students.AnyAsync(student => student.Id == studentId);
    }

    private Task<bool> IsCourseNameAvailableAsync(string name, int? courseIdToIgnore = null)
    {
        var normalizedName = name.ToLower();
        return db.Courses.AllAsync(course =>
            course.Name.ToLower() != normalizedName || course.Id == courseIdToIgnore);
    }

    private static async Task<PagedResult<T>> CreatePageAsync<T>(
        IQueryable<T> query,
        int pageNumber,
        int pageSize)
    {
        var safePageSize = Math.Clamp(pageSize, 1, 100);
        var totalCount = await query.CountAsync();
        var totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)safePageSize));
        var currentPage = Math.Clamp(pageNumber, 1, totalPages);
        var items = await query
            .Skip((currentPage - 1) * safePageSize)
            .Take(safePageSize)
            .ToListAsync();

        return new PagedResult<T>
        {
            Items = items,
            PageNumber = currentPage,
            TotalPages = totalPages,
            TotalCount = totalCount
        };
    }
}
