namespace razor_exercise.Models;

// An enrollment connects one student to one course.
public class Enrollment
{
    public int StudentId { get; set; }

    public int CourseId { get; set; }
}
