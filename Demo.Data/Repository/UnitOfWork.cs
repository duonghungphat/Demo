using Demo.Data;
using Demo.Models;
using Microsoft.EntityFrameworkCore.Storage;

namespace Demo.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        private IDbContextTransaction? _transaction;

        public IRepository<Category> Categories { get; }

        public IRepository<Expense> Expenses { get; }

        public IRepository<CourseCategory> CourseCategories { get; }

        public IRepository<Instructor> Instructors { get; }

        public IRepository<Course> Courses { get; }

        public IRepository<Lesson> Lessons { get; }

        public UnitOfWork(AppDbContext context, IRepository<Category> categories, IRepository<Expense> expenses, IRepository<CourseCategory> courseCategories, IRepository<Instructor> instructors, IRepository<Course> courses, IRepository<Lesson> lessons)
        {
            _context = context;

            Categories = categories;
            Expenses = expenses;
            CourseCategories = courseCategories;
            Instructors = instructors;
            Courses = courses;
            Lessons = lessons;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            if (_transaction != null)
                return;

            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction == null)
                return;

            await _transaction.CommitAsync();

            await _transaction.DisposeAsync();

            _transaction = null;
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction == null)
                return;

            await _transaction.RollbackAsync();

            await _transaction.DisposeAsync();

            _transaction = null;
        }
    }
}