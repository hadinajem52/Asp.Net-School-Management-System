using Microsoft.EntityFrameworkCore;
using MVC_exercise.Models;

namespace MVC_exercise.Data;

public static class SchoolDataSeeder
{
    public static async Task SeedAsync(SchoolDbContext db)
    {
        if (!await db.Students.AnyAsync())
        {
            db.Students.AddRange(
                new Student { FirstName = "Olivia", LastName = "Bennett", Email = "olivia.bennett@example.com", DateOfBirth = new DateTime(2004, 3, 14) },
                new Student { FirstName = "Ethan", LastName = "Carter", Email = "ethan.carter@example.com", DateOfBirth = new DateTime(2003, 9, 22) },
                new Student { FirstName = "Sophia", LastName = "Mitchell", Email = "sophia.mitchell@example.com", DateOfBirth = new DateTime(2005, 1, 8) },
                new Student { FirstName = "Liam", LastName = "Parker", Email = "liam.parker@example.com", DateOfBirth = new DateTime(2002, 7, 30) },
                new Student { FirstName = "Ava", LastName = "Collins", Email = "ava.collins@example.com", DateOfBirth = new DateTime(2004, 11, 5) },
                new Student { FirstName = "Noah", LastName = "Reed", Email = "noah.reed@example.com", DateOfBirth = new DateTime(2003, 5, 19) });
        }

        if (!await db.Courses.AnyAsync())
        {
            db.Courses.AddRange(
                new Course { Name = "Foundations of C#", Description = "Variables, decisions, loops, and methods.", Credits = 3 },
                new Course { Name = "Web Development Basics", Description = "Build server-rendered web applications with ASP.NET Core.", Credits = 4 },
                new Course { Name = "Relational Databases", Description = "Design tables, keys, and queries with PostgreSQL.", Credits = 3 },
                new Course { Name = "Human-Centered Design", Description = "Create clear interfaces around people and their goals.", Credits = 2 },
                new Course { Name = "Algorithms", Description = "Practice problem solving with common algorithm patterns.", Credits = 3 });
        }

        await db.SaveChangesAsync();

        if (await db.Enrollments.AnyAsync())
        {
            return;
        }

        var students = await db.Students.ToDictionaryAsync(student => student.Email);
        var courses = await db.Courses.ToDictionaryAsync(course => course.Name);

        db.Enrollments.AddRange(
            CreateEnrollment(students, courses, "olivia.bennett@example.com", "Foundations of C#"),
            CreateEnrollment(students, courses, "olivia.bennett@example.com", "Web Development Basics"),
            CreateEnrollment(students, courses, "ethan.carter@example.com", "Relational Databases"),
            CreateEnrollment(students, courses, "sophia.mitchell@example.com", "Web Development Basics"),
            CreateEnrollment(students, courses, "liam.parker@example.com", "Algorithms"),
            CreateEnrollment(students, courses, "ava.collins@example.com", "Human-Centered Design"),
            CreateEnrollment(students, courses, "noah.reed@example.com", "Foundations of C#"));

        await db.SaveChangesAsync();
    }

    private static Enrollment CreateEnrollment(
        IReadOnlyDictionary<string, Student> students,
        IReadOnlyDictionary<string, Course> courses,
        string studentEmail,
        string courseName)
    {
        return new Enrollment
        {
            StudentId = students[studentEmail].Id,
            CourseId = courses[courseName].Id
        };
    }
}
