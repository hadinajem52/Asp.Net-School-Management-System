using razor_exercise.Models;

namespace razor_exercise.Data;

public static class DatabaseSeeder
{
    public static void Seed(SchoolDbContext db)
    {
        if (db.Students.Any() || db.Courses.Any() || db.Enrollments.Any())
        {
            return;
        }

        List<Student> students =
        [
            new() { FirstName = "Olivia", LastName = "Bennett", Email = "olivia.bennett@example.com", DateOfBirth = new DateTime(2004, 3, 14) },
            new() { FirstName = "Ethan", LastName = "Carter", Email = "ethan.carter@example.com", DateOfBirth = new DateTime(2003, 9, 22) },
            new() { FirstName = "Sophia", LastName = "Mitchell", Email = "sophia.mitchell@example.com", DateOfBirth = new DateTime(2005, 1, 8) },
            new() { FirstName = "Liam", LastName = "Parker", Email = "liam.parker@example.com", DateOfBirth = new DateTime(2002, 7, 30) },
            new() { FirstName = "Ava", LastName = "Collins", Email = "ava.collins@example.com", DateOfBirth = new DateTime(2004, 11, 5) },
            new() { FirstName = "Noah", LastName = "Reed", Email = "noah.reed@example.com", DateOfBirth = new DateTime(2003, 5, 19) }
        ];

        List<Course> courses =
        [
            new() { Name = "Foundations of C#", Description = "Variables, decisions, loops, and methods.", Credits = 3 },
            new() { Name = "Web Development Basics", Description = "Build server-rendered web applications with ASP.NET Core.", Credits = 4 },
            new() { Name = "Relational Databases", Description = "Design tables, keys, and queries with PostgreSQL.", Credits = 3 },
            new() { Name = "Human-Centered Design", Description = "Create clear interfaces around people and their goals.", Credits = 2 },
            new() { Name = "Algorithms", Description = "Practice problem solving with common algorithm patterns.", Credits = 3 }
        ];

        db.Students.AddRange(students);
        db.Courses.AddRange(courses);
        db.SaveChanges();

        db.Enrollments.AddRange(
            new Enrollment { StudentId = students[0].Id, CourseId = courses[0].Id },
            new Enrollment { StudentId = students[0].Id, CourseId = courses[1].Id },
            new Enrollment { StudentId = students[1].Id, CourseId = courses[2].Id },
            new Enrollment { StudentId = students[2].Id, CourseId = courses[1].Id },
            new Enrollment { StudentId = students[3].Id, CourseId = courses[4].Id },
            new Enrollment { StudentId = students[4].Id, CourseId = courses[3].Id },
            new Enrollment { StudentId = students[5].Id, CourseId = courses[0].Id });

        db.SaveChanges();
    }
}
