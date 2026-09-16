using Demo.Models;
using Demo.ViewModels;
using Demo.Repositories;
using Microsoft.EntityFrameworkCore;
using Demo.Service.Services.Interfaces;

namespace Demo.Service.Services.Implementations
{
    public class CourseService : ICourseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICoursePriceCalculator _priceCalculator;
        private readonly ICoursePricingPolicy _pricingPolicy;

        public CourseService(IUnitOfWork unitOfWork, ICoursePriceCalculator priceCalculator, ICoursePricingPolicy pricingPolicy)
        {
            _unitOfWork = unitOfWork;
            _priceCalculator = priceCalculator;
            _pricingPolicy = pricingPolicy;
        }

        public async Task<List<CourseViewModel>> GetAllAsync(string? keyword = null)
        {
            var query = _unitOfWork.Courses
                .BuildQuery(x => true);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(x => x.Title.Contains(keyword));
            }

            return await query
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new CourseViewModel
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description,
                    Price = x.Price,
                    Level = x.Level,
                    Status = x.Status,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    CourseCategoryId = x.CourseCategoryId,
                    InstructorId = x.InstructorId,
                    ImagePath = x.ImagePath,
                    DiscountPercent = x.DiscountPercent,
                    CourseCategoryName = x.CourseCategory.Name,
                    InstructorName = x.Instructor.FullName
                })
                .ToListAsync();
        }

        public async Task<CourseViewModel?> GetByIdAsync(int id)
        {
            var model = await _unitOfWork.Courses
                .BuildQuery(x => x.Id == id)
                .Select(x => new CourseViewModel
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description,
                    Price = x.Price,
                    Level = x.Level,
                    Status = x.Status,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    CourseCategoryId = x.CourseCategoryId,
                    InstructorId = x.InstructorId,
                    ImagePath = x.ImagePath,
                    DiscountPercent = x.DiscountPercent,

                    CourseCategoryName = x.CourseCategory.Name,
                    InstructorName = x.Instructor.FullName,

                    Lessons = x.Lessons
                        .Where(l => !l.IsDeleted)
                        .OrderBy(l => l.Order)
                        .Select(l => new LessonViewModel
                        {
                            Id = l.Id,
                            CourseId = l.CourseId,
                            Title = l.Title,
                            Description = l.Description,
                            Order = l.Order,
                            DurationMinutes = l.DurationMinutes
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();

            if (model != null)
            {
                model.FinalPrice =
                    _priceCalculator.CalculateFinalPrice(
                        model.Price,
                        model.DiscountPercent);
            }

            return model;
        }
        public async Task CreateAsync(CourseViewModel model)
        {
            await ValidateCourseAsync(model);

            var course = new Course
            {
                Title = model.Title,
                Description = model.Description,
                Price = model.Price,
                Level = model.Level,
                StartDate = model.StartDate!.Value,
                EndDate = model.EndDate!.Value,
                CourseCategoryId = model.CourseCategoryId,
                InstructorId = model.InstructorId,
                ImagePath = model.ImagePath,
                DiscountPercent = model.DiscountPercent,

                Status = CourseStatus.Draft,
                CreatedAt = DateTime.Now
            };

            await _unitOfWork.Courses.AddAsync(course);

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateAsync(CourseViewModel model)
        {
            await ValidateCourseAsync(model);

            var existing = await _unitOfWork.Courses.GetByIdAsync(model.Id);

            if (existing == null)
            {
                throw new KeyNotFoundException("Course not found.");
            }

            existing.Title = model.Title;
            existing.Description = model.Description;
            existing.Price = model.Price;
            existing.Level = model.Level;
            existing.StartDate = model.StartDate!.Value;
            existing.EndDate = model.EndDate!.Value;
            existing.CourseCategoryId = model.CourseCategoryId;
            existing.InstructorId = model.InstructorId;
            existing.ImagePath = model.ImagePath;
            existing.DiscountPercent = model.DiscountPercent;

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var course = await _unitOfWork.Courses.GetByIdAsync(id);

                if (course == null)
                {
                    throw new KeyNotFoundException("Course not found.");
                }

                var lessons = await _unitOfWork.Lessons
                    .BuildQuery(x => x.CourseId == id)
                    .ToListAsync();

                foreach (var lesson in lessons)
                {
                    _unitOfWork.Lessons.Delete(lesson);
                }

                await _unitOfWork.SaveChangesAsync();

                _unitOfWork.Courses.Delete(course);

                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task PublishAsync(int id)
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(id);

            if (course == null)
            {
                throw new KeyNotFoundException("Course not found.");
            }

            if (course.Status != CourseStatus.Draft)
            {
                throw new InvalidOperationException("Only draft courses can be published.");
            }

            var hasLessons = await _unitOfWork.Lessons
               .BuildQuery(x => x.CourseId == id)
               .AnyAsync();

            if (!hasLessons)
            {
                throw new InvalidOperationException("Course must have at least one lesson before publishing.");
            }

            course.Status = CourseStatus.Published;

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task CloseAsync(int id)
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(id);

            if (course == null)
            {
                throw new KeyNotFoundException("Course not found.");
            }

            if (course.Status != CourseStatus.Published)
            {
                throw new InvalidOperationException("Only published courses can be closed.");
            }

            course.Status = CourseStatus.Closed;

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<LessonViewModel?> GetLessonByIdAsync(int lessonId)
        {
            return await _unitOfWork.Lessons
                .BuildQuery(x => x.Id == lessonId)
                .Select(x => new LessonViewModel
                {
                    Id = x.Id,
                    CourseId = x.CourseId,
                    Title = x.Title,
                    Description = x.Description,
                    Order = x.Order,
                    DurationMinutes = x.DurationMinutes
                })
                .FirstOrDefaultAsync();
        }

        public async Task AddLessonAsync(LessonViewModel model)
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(model.CourseId);

            if (course == null)
            {
                throw new KeyNotFoundException("Course not found.");
            }

            var orderExists = await _unitOfWork.Lessons
                .BuildQuery(x =>
                    x.CourseId == model.CourseId &&
                    x.Order == model.Order)
                .AnyAsync();

            if (orderExists)
            {
                throw new InvalidOperationException("Lesson order already exists in this course.");
            }

            var lesson = new Lesson
            {
                CourseId = model.CourseId,
                Title = model.Title,
                Description = model.Description,
                Order = model.Order,
                DurationMinutes = model.DurationMinutes
            };

            await _unitOfWork.Lessons.AddAsync(lesson);

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateLessonAsync(LessonViewModel model)
        {
            var existing =
                await _unitOfWork.Lessons.GetByIdAsync(model.Id);

            if (existing == null)
            {
                throw new KeyNotFoundException("Lesson not found.");
            }

            var orderExists = await _unitOfWork.Lessons
                .BuildQuery(x =>
                    x.CourseId == existing.CourseId &&
                    x.Order == model.Order &&
                    x.Id != model.Id)
                .AnyAsync();

            if (orderExists)
            {
                throw new InvalidOperationException("Lesson order already exists in this course.");
            }

            existing.Title = model.Title;
            existing.Description = model.Description;
            existing.Order = model.Order;
            existing.DurationMinutes = model.DurationMinutes;

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteLessonAsync(int lessonId)
        {
            var lesson = await _unitOfWork.Lessons.GetByIdAsync(lessonId);

            if (lesson == null)
            {
                throw new KeyNotFoundException("Lesson not found.");
            }

            _unitOfWork.Lessons.Delete(lesson);

            await _unitOfWork.SaveChangesAsync();
        }

        private async Task ValidateCourseAsync(CourseViewModel model)
        {
            if (model.Price < 0)
            {
                throw new InvalidOperationException("Course price cannot be negative.");
            }

            if (!model.StartDate.HasValue || !model.EndDate.HasValue)
            {
                throw new InvalidOperationException("Start date and end date are required.");
            }

            if (model.EndDate <= model.StartDate)
            {
                throw new InvalidOperationException("End date must be later than start date.");
            }

            if (!await _unitOfWork.CourseCategories
                .ExistsAsync(model.CourseCategoryId))
            {
                throw new InvalidOperationException("Selected course category does not exist.");
            }

            if (!await _unitOfWork.Instructors.ExistsAsync(model.InstructorId))
            {
                throw new InvalidOperationException("Selected instructor does not exist.");
            }

            if (!_pricingPolicy.IsDiscountAllowed(model.DiscountPercent))
            {
                throw new InvalidOperationException($"Discount must be between 0 and {_pricingPolicy.MaxDiscountPercent}%.");
            }
        }

        public decimal CalculateFinalPrice(CourseViewModel model)
        {
            return _priceCalculator.CalculateFinalPrice(model.Price, model.DiscountPercent);
        }
    }
}