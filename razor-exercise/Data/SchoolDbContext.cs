using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using razor_exercise.Models;

namespace razor_exercise.Data;

// object the app uses to interact with the database. 

public class SchoolDbContext(DbContextOptions<SchoolDbContext> options)
     : IdentityDbContext<ApplicationUser>(options)

{

    // we declaredb the three DB tables
    public DbSet<Student> Students => Set<Student>();

    public DbSet<Course> Courses => Set<Course>();

    public DbSet<Enrollment> Enrollments => Set<Enrollment>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        // lets IdentityDbContext configure its Identity database rules before we configure our own rules.
        base.OnModelCreating(modelBuilder);

        // we are configuring the model to enforce unique constraints on the Name property of the Course
        modelBuilder.Entity<Course>()
            .HasIndex(course => course.Name)
            .IsUnique();

        // A date of birth has no time or time-zone part. Keep the existing PostgreSQL `date` column type.
        modelBuilder.Entity<Student>()
            .Property(student => student.DateOfBirth)
            .HasColumnType("date");

        // we are configuring the model to define a composite primary key for the Enrollment entity
        modelBuilder.Entity<Enrollment>()
            .HasKey(enrollment => new { enrollment.StudentId, enrollment.CourseId });


        // we are configuring the model to define the relationships between the Student, Course, and Enrollment entities.
        modelBuilder.Entity<Enrollment>()
            .HasOne<Student>()
            .WithMany()
            .HasForeignKey(enrollment => enrollment.StudentId)
            
            // when a Student is deleted, all related Enrollment records will also be deleted from the database. 
            .OnDelete(DeleteBehavior.Cascade);


        // we are configuring the model to define the relationships between the Student, Course, and Enrollment entities.
        modelBuilder.Entity<Enrollment>()
            .HasOne<Course>()
            .WithMany()
            .HasForeignKey(enrollment => enrollment.CourseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
