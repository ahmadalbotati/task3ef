using Microsoft.EntityFrameworkCore;

public class Program
{
    public static void Main()
    {
        AddInstructor();
        AddCourse("C# Fundamentals", 450, 1, 3);
        AddStudents();
        EnrollStudent(4, 1);

        FindCourse(1, "Advanced EF Core");
        FilterCourses(200, 600);
        ShowSubCategories(1);
        ShowRoster(1);

        ChangePrice(3, 350);
        UpdateProgress(4, 1, 100);
        PublishCourse(3);

        Unenroll(1, 2);
        DeleteCategoryUnsafe(1);
        DeleteCategorySafe(1);
        DeleteCourse(1);
    }

    static void AddInstructor()
    {
        using var db = new AppDbContext();
        var instructor = new Instructor
        {
            FullName = "Khaled Mostafa",
            Email = "khaled@mail.com",
            JoinedDate = DateTime.Now
        };
        db.Instructors.Add(instructor);
        db.SaveChanges();
        Console.WriteLine($"Instructor added. Id = {instructor.Id}");
    }

    static void AddCourse(string title, decimal price, int instructorId, int categoryId)
    {
        using var db = new AppDbContext();
        if (!db.Instructors.Any(i => i.Id == instructorId))
        {
            Console.WriteLine("Instructor not found.");
            return;
        }
        if (!db.Categories.Any(c => c.Id == categoryId))
        {
            Console.WriteLine("Category not found.");
            return;
        }
        var course = new Course
        {
            Title = title,
            Price = price,
            Level = CourseLevel.Beginner,
            IsPublished = false,
            InstructorId = instructorId,
            CategoryId = categoryId
        };
        db.Courses.Add(course);
        db.SaveChanges();
        Console.WriteLine($"Course added. Id = {course.Id}");
    }

    static void AddStudents()
    {
        using var db = new AppDbContext();
        var students = new List<Student>
        {
            new Student { FullName = "Hana Tarek", Email = "hana@mail.com", DateOfBirth = new DateTime(2002, 2, 2) },
            new Student { FullName = "Karim Nabil", Email = "karim@mail.com", DateOfBirth = new DateTime(2001, 5, 15) },
            new Student { FullName = "Nour Ehab", Email = "nour@mail.com", DateOfBirth = new DateTime(2003, 8, 8) }
        };
        db.Students.AddRange(students);
        db.SaveChanges();
        foreach (var s in students)
            Console.WriteLine($"Student added. Id = {s.Id}");
    }

    static void EnrollStudent(int studentId, int courseId)
    {
        using var db = new AppDbContext();
        if (!db.Students.Any(s => s.Id == studentId))
        {
            Console.WriteLine("Enrollment refused: student not found.");
            return;
        }
        var course = db.Courses.Find(courseId);
        if (course == null)
        {
            Console.WriteLine("Enrollment refused: course not found.");
            return;
        }
        if (!course.IsPublished)
        {
            Console.WriteLine("Enrollment refused: course is not published.");
            return;
        }
        if (db.Enrollments.Any(e => e.StudentId == studentId && e.CourseId == courseId))
        {
            Console.WriteLine("Enrollment refused: student is already enrolled.");
            return;
        }
        db.Enrollments.Add(new Enrollment
        {
            StudentId = studentId,
            CourseId = courseId,
            EnrollmentDate = DateTime.Now,
            Progress = 0,
            Status = EnrollmentStatus.Active
        });
        db.SaveChanges();
        Console.WriteLine("Student enrolled.");
    }

    static void FindCourse(int id, string title)
    {
        using var db = new AppDbContext();
        var byId = db.Courses.Find(id);
        Console.WriteLine(byId == null ? "Course not found." : $"By Id: {byId.Title}");

        var byTitle = db.Courses.FirstOrDefault(c => c.Title == title);
        Console.WriteLine(byTitle == null ? "Course not found." : $"By title: {byTitle.Title}");
    }

    static void FilterCourses(decimal min, decimal max)
    {
        using var db = new AppDbContext();
        var courses = db.Courses
            .Where(c => c.Price >= min && c.Price <= max)
            .OrderByDescending(c => c.Price)
            .Select(c => new { c.Title, c.Level, c.Price })
            .ToList();
        foreach (var c in courses)
            Console.WriteLine($"{c.Title} | {c.Level} | {c.Price}");
    }

    static void ShowSubCategories(int categoryId)
    {
        using var db = new AppDbContext();
        var subs = db.Categories.Where(c => c.ParentCategoryId == categoryId).ToList();
        Console.WriteLine("Sub-categories:");
        foreach (var c in subs)
            Console.WriteLine(c.Name);

        var topLevel = db.Categories.Where(c => c.ParentCategoryId == null).ToList();
        Console.WriteLine("Top-level categories:");
        foreach (var c in topLevel)
            Console.WriteLine(c.Name);
    }

    static void ShowRoster(int courseId)
    {
        using var db = new AppDbContext();
        var studentIds = db.Enrollments
            .Where(e => e.CourseId == courseId)
            .Select(e => e.StudentId)
            .ToList();
        Console.WriteLine($"Enrolled students: {studentIds.Count}");

        var students = db.Students.Where(s => studentIds.Contains(s.Id)).ToList();
        foreach (var s in students)
            Console.WriteLine(s.FullName);
    }

    static void ChangePrice(int courseId, decimal newPrice)
    {
        using var db = new AppDbContext();
        var course = db.Courses.Find(courseId);
        if (course == null)
        {
            Console.WriteLine("Course not found.");
            return;
        }
        course.Price = newPrice;
        db.SaveChanges();
        Console.WriteLine("Price updated.");
    }

    static void UpdateProgress(int studentId, int courseId, decimal progress)
    {
        if (progress < 0 || progress > 100)
        {
            Console.WriteLine("Progress must be between 0 and 100.");
            return;
        }
        using var db = new AppDbContext();
        var enrollment = db.Enrollments.Find(studentId, courseId);
        if (enrollment == null)
        {
            Console.WriteLine("Enrollment not found.");
            return;
        }
        enrollment.Progress = progress;
        if (progress == 100)
            enrollment.Status = EnrollmentStatus.Completed;
        db.SaveChanges();
        Console.WriteLine("Progress updated.");
    }

    static void PublishCourse(int courseId)
    {
        using var db = new AppDbContext();
        var course = db.Courses.Find(courseId);
        if (course == null)
        {
            Console.WriteLine("Course not found.");
            return;
        }
        course.IsPublished = true;
        course.PublishedDate = DateTime.Now;
        db.SaveChanges();
        Console.WriteLine("Course published.");
    }

    static void Unenroll(int studentId, int courseId)
    {
        using var db = new AppDbContext();
        var enrollment = db.Enrollments.Find(studentId, courseId);
        if (enrollment == null)
        {
            Console.WriteLine("Enrollment not found.");
            return;
        }
        db.Enrollments.Remove(enrollment);
        db.SaveChanges();
        Console.WriteLine("Student unenrolled.");
    }

    static void DeleteCategoryUnsafe(int categoryId)
    {
        using var db = new AppDbContext();
        var category = db.Categories.Find(categoryId);
        if (category == null)
        {
            Console.WriteLine("Category not found.");
            return;
        }
        try
        {
            db.Categories.Remove(category);
            db.SaveChanges();
            Console.WriteLine("Category deleted.");
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine(ex.InnerException?.Message ?? ex.Message);
        }
    }

    static void DeleteCategorySafe(int categoryId)
    {
        using var db = new AppDbContext();
        var category = db.Categories.Find(categoryId);
        if (category == null)
        {
            Console.WriteLine("Category not found.");
            return;
        }
        if (db.Categories.Any(c => c.ParentCategoryId == categoryId))
        {
            Console.WriteLine("Cannot delete: category still has sub-categories.");
            return;
        }
        db.Categories.Remove(category);
        db.SaveChanges();
        Console.WriteLine("Category deleted.");
    }

    static void DeleteCourse(int courseId)
    {
        using var db = new AppDbContext();
        var course = db.Courses.Find(courseId);
        if (course == null)
        {
            Console.WriteLine("Course not found.");
            return;
        }
        int before = db.Enrollments.Count(e => e.CourseId == courseId);
        Console.WriteLine($"Enrollments before: {before}");

        db.Courses.Remove(course);
        db.SaveChanges();

        int after = db.Enrollments.Count(e => e.CourseId == courseId);
        Console.WriteLine($"Enrollments after: {after}");
        // The enrollments were removed because the Enrollment-Course relationship uses Cascade delete, so the database deletes the dependent enrollments when the course is deleted.
    }
}