using Microsoft.EntityFrameworkCore;

public static class SeedData
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Instructor>().HasData(
            new Instructor { Id = 1, FullName = "Ahmed Hassan", Email = "ahmed@mail.com", Bio = "Backend developer", JoinedDate = new DateTime(2024, 1, 10) },
            new Instructor { Id = 2, FullName = "Sara Ali", Email = "sara@mail.com", JoinedDate = new DateTime(2024, 3, 5) }
        );

        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Programming" },
            new Category { Id = 2, Name = "Backend", ParentCategoryId = 1 },
            new Category { Id = 3, Name = "ASP.NET", ParentCategoryId = 2 },
            new Category { Id = 4, Name = "Design" }
        );

        modelBuilder.Entity<Course>().HasData(
            new Course { Id = 1, Title = "ASP.NET Core Basics", Price = 500, Level = CourseLevel.Beginner, IsPublished = true, PublishedDate = new DateTime(2024, 5, 1), InstructorId = 1, CategoryId = 3 },
            new Course { Id = 2, Title = "Advanced EF Core", Price = 800, Level = CourseLevel.Advanced, IsPublished = true, PublishedDate = new DateTime(2024, 6, 1), InstructorId = 1, CategoryId = 3 },
            new Course { Id = 3, Title = "UI Design", Price = 300, Level = CourseLevel.Beginner, IsPublished = false, InstructorId = 2, CategoryId = 4 }
        );

        modelBuilder.Entity<Student>().HasData(
            new Student { Id = 1, FullName = "Omar Khaled", Email = "omar@mail.com", DateOfBirth = new DateTime(2002, 4, 12) },
            new Student { Id = 2, FullName = "Mona Samir", Email = "mona@mail.com", DateOfBirth = new DateTime(2001, 9, 30) },
            new Student { Id = 3, FullName = "Youssef Adel", Email = "youssef@mail.com", DateOfBirth = new DateTime(2003, 1, 20) }
        );

        modelBuilder.Entity<Enrollment>().HasData(
            new Enrollment { StudentId = 1, CourseId = 1, EnrollmentDate = new DateTime(2024, 7, 1), Progress = 40, Status = EnrollmentStatus.Active },
            new Enrollment { StudentId = 2, CourseId = 1, EnrollmentDate = new DateTime(2024, 7, 2), Progress = 100, Status = EnrollmentStatus.Completed, FinalGrade = 90 },
            new Enrollment { StudentId = 1, CourseId = 2, EnrollmentDate = new DateTime(2024, 7, 3), Progress = 10, Status = EnrollmentStatus.Active }
        );
    }
}