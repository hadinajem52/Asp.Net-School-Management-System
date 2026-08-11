using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MVC_exercise.Models;

namespace MVC_exercise.Data;

public class SchoolDbContext(DbContextOptions<SchoolDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{

    public DbSet<Student> Students => Set<Student>();

    public DbSet<Course> Courses => Set<Course>();

    public DbSet<Enrollment> Enrollments => Set<Enrollment>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        base.OnModelCreating(modelBuilder);


        modelBuilder.Entity<Student>()
            .Property(student => student.DateOfBirth)
            .HasColumnType("date");

        modelBuilder.Entity<Course>()
            .HasIndex(course => course.Name)
            .IsUnique();

        modelBuilder.Entity<Enrollment>()
            .HasKey(enrollment => new { enrollment.StudentId, enrollment.CourseId });

        modelBuilder.Entity<Enrollment>()
            .HasOne<Student>()
            .WithMany()
            .HasForeignKey(enrollment => enrollment.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Enrollment>()
            .HasOne<Course>()
            .WithMany()
            .HasForeignKey(enrollment => enrollment.CourseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
