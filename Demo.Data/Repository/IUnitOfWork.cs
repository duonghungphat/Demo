using Demo.Models;

namespace Demo.Repositories
{
    public interface IUnitOfWork
    {
        IRepository<Category> Categories { get; }

        IRepository<Expense> Expenses { get; }

        IRepository<CourseCategory> CourseCategories { get; }

        IRepository<Instructor> Instructors { get; }

        IRepository<Course> Courses { get; }

        IRepository<Lesson> Lessons { get; }

        Task<int> SaveChangesAsync();

        Task BeginTransactionAsync();

        Task CommitTransactionAsync();

        Task RollbackTransactionAsync();
    }
}