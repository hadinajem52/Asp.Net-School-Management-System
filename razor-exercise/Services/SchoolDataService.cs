using Microsoft.EntityFrameworkCore;
using razor_exercise.Data;
using razor_exercise.Models;

namespace razor_exercise.Services;

public class SchoolDataService(SchoolDbContext db)
{

    // Dashboard summary values. CountAsync lets PostgreSQL do the counting.
    public Task<int> GetStudentCountAsync()
    {
        return db.Students.CountAsync();
    }

    public Task<int> GetCourseCountAsync()
    {
        return db.Courses.CountAsync();
    }

    // A higher database ID means the row was added later in this application.
    public Task<List<Student>> GetMostRecentStudentsAsync()
    {
        return db.Students
            .AsNoTracking()
            .OrderByDescending(student => student.Id)
            .Take(5)
            .ToListAsync();
    }

    // this method retrieves a list of all students from the database,
    // ordered by last name and then first name.
    public Task<List<Student>> GetStudentsAsync(
        string? searchTerm = null,
        string? sortBy = null)
    {
        IQueryable<Student> students = db.Students.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            string searchPattern = $"%{searchTerm.Trim()}%";

            students = students.Where(student =>
                EF.Functions.ILike(student.FirstName, searchPattern) ||
                EF.Functions.ILike(student.LastName, searchPattern));
        }

        return sortBy switch
        {
            "FirstName" => students
                .OrderBy(student => student.FirstName)
                .ThenBy(student => student.LastName)
                .ToListAsync(),

            "LastName" => students
                .OrderBy(student => student.LastName)
                .ThenBy(student => student.FirstName)
                .ToListAsync(),

            "DateOfBirth" => students
                .OrderBy(student => student.DateOfBirth)
                .ToListAsync(),

            _ => students
                .OrderBy(student => student.LastName)
                .ThenBy(student => student.FirstName)
                .ToListAsync()
        };
    }

    public async Task<PagedResult<Student>> GetStudentsPageAsync(
        string? searchTerm,
        string? sortBy,
        int pageNumber,
        int pageSize)
    {
        IQueryable<Student> students = db.Students.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            string searchPattern = $"%{searchTerm.Trim()}%";
            students = students.Where(student =>
                EF.Functions.ILike(student.FirstName, searchPattern) ||
                EF.Functions.ILike(student.LastName, searchPattern));
        }

        IOrderedQueryable<Student> orderedStudents = sortBy switch
        {
            "FirstName" => students.OrderBy(student => student.FirstName).ThenBy(student => student.LastName),
            "LastName" => students.OrderBy(student => student.LastName).ThenBy(student => student.FirstName),
            "DateOfBirth" => students.OrderBy(student => student.DateOfBirth),
            _ => students.OrderBy(student => student.LastName).ThenBy(student => student.FirstName)
        };

        int totalCount = await students.CountAsync();
        int safePageSize = Math.Max(pageSize, 1);
        int totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)safePageSize));
        int currentPage = Math.Clamp(pageNumber, 1, totalPages);

        List<Student> items = await orderedStudents
            .Skip((currentPage - 1) * safePageSize)
            .Take(safePageSize)
            .ToListAsync();

        return new PagedResult<Student>
        {
            Items = items,
            PageNumber = currentPage,
            TotalPages = totalPages,
            TotalCount = totalCount
        };
    }

    // get one student by id
    public Task<Student?> GetStudentAsync(int id)
    {
        return db.Students
            .AsNoTracking()
            .FirstOrDefaultAsync(student => student.Id == id);
    }

    // get one student by email
    public async Task AddStudentAsync(Student student)
    {
        db.Students.Add(student);
        //the method must wait for the changes to be saved to the database before returning, so we use await here.
        await db.SaveChangesAsync();
    }

    // update a student by id, if the student does not exist, return false
    public async Task<bool> UpdateStudentAsync(Student updatedStudent)
    {
        Student? existingStudent = await db.Students.FindAsync(updatedStudent.Id);

        if (existingStudent is null)
        {
            return false;
        }

        existingStudent.FirstName = updatedStudent.FirstName;
        existingStudent.LastName = updatedStudent.LastName;
        existingStudent.Email = updatedStudent.Email;
        existingStudent.DateOfBirth = updatedStudent.DateOfBirth;
        await db.SaveChangesAsync();
        return true;
    }

    // delete a student by id, if the student does not exist, return false
    public async Task<bool> DeleteStudentAsync(int studentId)
    {
        
        Student? student = await db.Students.FindAsync(studentId);

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
        IQueryable<Course> courses = db.Courses.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            string searchPattern = $"%{searchTerm.Trim()}%";
            courses = courses.Where(course => EF.Functions.ILike(course.Name, searchPattern));
        }

        return courses
            .OrderBy(course => course.Name)
            .ToListAsync();
    }

    public async Task<PagedResult<Course>> GetCoursesPageAsync(
        string? searchTerm,
        int pageNumber,
        int pageSize)
    {
        IQueryable<Course> courses = db.Courses.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            string searchPattern = $"%{searchTerm.Trim()}%";
            courses = courses.Where(course => EF.Functions.ILike(course.Name, searchPattern));
        }

        int totalCount = await courses.CountAsync();
        int safePageSize = Math.Max(pageSize, 1);
        int totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)safePageSize));
        int currentPage = Math.Clamp(pageNumber, 1, totalPages);

        List<Course> items = await courses
            .OrderBy(course => course.Name)
            .Skip((currentPage - 1) * safePageSize)
            .Take(safePageSize)
            .ToListAsync();

        return new PagedResult<Course>
        {
            Items = items,
            PageNumber = currentPage,
            TotalPages = totalPages,
            TotalCount = totalCount
        };
    }


    public Task<Course?> GetCourseAsync(int id)
    {
        return db.Courses
            .AsNoTracking()
            .FirstOrDefaultAsync(course => course.Id == id);
    }

    public async Task<bool> AddCourseAsync(Course course)
    {
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
        Course? existingCourse = await db.Courses.FindAsync(updatedCourse.Id);

        if (existingCourse is null || !await IsCourseNameAvailableAsync(updatedCourse.Name, updatedCourse.Id))
        {
            return false;
        }

        existingCourse.Name = updatedCourse.Name;
        existingCourse.Description = updatedCourse.Description;
        existingCourse.Credits = updatedCourse.Credits;
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteCourseAsync(int courseId)
    {
        Course? course = await db.Courses.FindAsync(courseId);

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
        bool studentExists = await db.Students.AnyAsync(student => student.Id == studentId);
        bool courseExists = await db.Courses.AnyAsync(course => course.Id == courseId);
        bool alreadyEnrolled = await db.Enrollments.AnyAsync(enrollment =>
            enrollment.StudentId == studentId && enrollment.CourseId == courseId);

        if (!studentExists || !courseExists || alreadyEnrolled)
        {
            return false;
        }

        db.Enrollments.Add(new Enrollment { StudentId = studentId, CourseId = courseId });
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> EnrollStudentInCoursesAsync(int studentId, IEnumerable<int> courseIds)
    {
        int[] selectedCourseIds = courseIds.Distinct().ToArray();

        if (selectedCourseIds.Length == 0)
        {
            return false;
        }

        bool studentExists = await db.Students.AnyAsync(student => student.Id == studentId);
        int matchingCourseCount = await db.Courses.CountAsync(course => selectedCourseIds.Contains(course.Id));
        bool isAlreadyEnrolled = await db.Enrollments.AnyAsync(enrollment =>
            enrollment.StudentId == studentId && selectedCourseIds.Contains(enrollment.CourseId));

        if (!studentExists || matchingCourseCount != selectedCourseIds.Length || isAlreadyEnrolled)
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
        Enrollment? enrollment = await db.Enrollments.FindAsync(studentId, courseId);

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

    private Task<bool> IsCourseNameAvailableAsync(string name, int? courseIdToIgnore = null)
    {
        return db.Courses.AllAsync(course =>
            course.Name != name || course.Id == courseIdToIgnore);
    }
}
