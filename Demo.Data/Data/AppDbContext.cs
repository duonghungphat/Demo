using Demo.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Demo.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Expense> Expenses { get; set; }

        public DbSet<CourseCategory> CourseCategories { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Lesson> Lessons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>()
                .Property(x => x.Name)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<Category>()
                .HasIndex(x => x.Name)
                .IsUnique();

            modelBuilder.Entity<Expense>()
                .Property(x => x.Note)
                .HasMaxLength(200)
                .IsRequired();

            modelBuilder.Entity<Expense>()
                .HasOne(x => x.Category)
                .WithMany(x => x.Expenses)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CourseCategory>()
                .Property(x => x.Name)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<CourseCategory>()
                .Property(x => x.Description)
                .HasMaxLength(300);

            modelBuilder.Entity<CourseCategory>()
                .HasIndex(x => x.Name)
                .IsUnique();

            modelBuilder.Entity<Instructor>()
                .Property(x => x.FullName)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<Instructor>()
                .Property(x => x.Email)
                .HasMaxLength(150)
                .IsRequired();

            modelBuilder.Entity<Instructor>()
                .Property(x => x.Phone)
                .HasMaxLength(20);

            modelBuilder.Entity<Instructor>()
                .Property(x => x.Bio)
                .HasMaxLength(500);

            modelBuilder.Entity<Instructor>()
                .HasIndex(x => x.Email)
                .IsUnique();

            modelBuilder.Entity<Course>()
                .Property(x => x.Title)
                .HasMaxLength(150)
                .IsRequired();

            modelBuilder.Entity<Course>()
                .Property(x => x.Description)
                .HasMaxLength(1000);

            modelBuilder.Entity<Course>()
                .Property(x => x.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Course>()
                .Property(x => x.ImagePath)
                .HasMaxLength(255);

            modelBuilder.Entity<Course>()
                .HasOne(x => x.CourseCategory)
                .WithMany(x => x.Courses)
                .HasForeignKey(x => x.CourseCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Course>()
                .HasOne(x => x.Instructor)
                .WithMany(x => x.Courses)
                .HasForeignKey(x => x.InstructorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Course>()
                .Property(x => x.DiscountPercent)
                .HasPrecision(5, 2);

            modelBuilder.Entity<Lesson>()
                .Property(x => x.Title)
                .HasMaxLength(150)
                .IsRequired();

            modelBuilder.Entity<Lesson>()
                .Property(x => x.Description)
                .HasMaxLength(1000);

            modelBuilder.Entity<Lesson>()
                .HasOne(x => x.Course)
                .WithMany(x => x.Lessons)
                .HasForeignKey(x => x.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Lesson>()
                .HasIndex(x => new { x.CourseId, x.Order })
                .IsUnique();
        }
    }
}