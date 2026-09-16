using Demo.Models;
using Demo.Repositories;
using Demo.ViewModels;
using Demo.Service.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Demo.Service.Services.Implementations
{
    public class CourseCategoryService : ICourseCategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CourseCategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<CourseCategoryViewModel>> GetAllAsync()
        {
            return await _unitOfWork.CourseCategories
                .BuildQuery(x => true)
                .OrderBy(x => x.Name)
                .Select(x => new CourseCategoryViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description
                })
                .ToListAsync();
        }

        public async Task<CourseCategoryViewModel?> GetByIdAsync(int id)
        {
            return await _unitOfWork.CourseCategories
                .BuildQuery(x => x.Id == id)
                .Select(x => new CourseCategoryViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description
                })
                .FirstOrDefaultAsync();
        }

        public async Task CreateAsync(CourseCategoryViewModel model)
        {
            var exists = await _unitOfWork.CourseCategories
                .BuildQuery(x => x.Name == model.Name)
                .AnyAsync();

            if (exists)
            {
                throw new InvalidOperationException("Course category name already exists.");
            }

            var courseCategory = new CourseCategory
            {
                Name = model.Name,
                Description = model.Description
            };

            await _unitOfWork.CourseCategories.AddAsync(courseCategory);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateAsync(CourseCategoryViewModel model)
        {
            var existing = await _unitOfWork.CourseCategories.GetByIdAsync(model.Id);

            if (existing == null)
            {
                throw new KeyNotFoundException("Course category not found.");
            }

            var exists = await _unitOfWork.CourseCategories
                .BuildQuery(x =>
                    x.Name == model.Name &&
                    x.Id != model.Id)
                .AnyAsync();

            if (exists)
            {
                throw new InvalidOperationException("Course category name already exists.");
            }

            existing.Name = model.Name;
            existing.Description = model.Description;

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var category = await _unitOfWork.CourseCategories.GetByIdAsync(id);

            if (category == null)
            {
                throw new KeyNotFoundException("Course category not found.");
            }

            var isUsed = await _unitOfWork.Courses
                .BuildQuery(x => x.CourseCategoryId == id)
                .AnyAsync();

            if (isUsed)
            {
                throw new InvalidOperationException("This course category is being used by a course.");
            }

            _unitOfWork.CourseCategories.Delete(category);

            await _unitOfWork.SaveChangesAsync();
        }
    }
}