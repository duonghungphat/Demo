using Demo.Models;
using Demo.Repositories;
using Demo.Service.Services.Interfaces;
using Demo.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Demo.Service.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<CategoryViewModel>> GetAllAsync()
        {
            return await _unitOfWork.Categories
                .BuildQuery(x => true)
                .OrderBy(x => x.Name)
                .Select(x => new CategoryViewModel
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .ToListAsync();
        }

        public async Task<CategoryViewModel?> GetByIdAsync(int id)
        {
            return await _unitOfWork.Categories
                .BuildQuery(x => x.Id == id)
                .Select(x => new CategoryViewModel
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .FirstOrDefaultAsync();
        }

        public async Task CreateAsync(CategoryViewModel model)
        {
            var exists = await _unitOfWork.Categories
                .BuildQuery(x => x.Name == model.Name)
                .AnyAsync();

            if (exists)
            {
                throw new InvalidOperationException("Category name already exists.");
            }

            var category = new Category
            {
                Name = model.Name
            };

            await _unitOfWork.Categories.AddAsync(category);

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateAsync(CategoryViewModel model)
        {
            var existing =
                await _unitOfWork.Categories
                    .GetByIdAsync(model.Id);

            if (existing == null)
            {
                throw new KeyNotFoundException("Category not found.");
            }

            var exists = await _unitOfWork.Categories
                .BuildQuery(x => x.Name == model.Name && x.Id != model.Id)
                .AnyAsync();

            if (exists)
            {
                throw new InvalidOperationException("Category name already exists.");
            }

            existing.Name = model.Name;

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);

            if (category == null)
            {
                throw new KeyNotFoundException("Category not found.");
            }

            var isUsed = await _unitOfWork.Expenses
                .BuildQuery(x => x.CategoryId == id)
                .AnyAsync();

            if (isUsed)
            {
                throw new InvalidOperationException("This category is being used by an expense.");
            }

            _unitOfWork.Categories.Delete(category);

            await _unitOfWork.SaveChangesAsync();
        }
        public async Task<List<CategoryViewModel>> GetDeletedAsync()
        {
            return await _unitOfWork.Categories
                .BuildQueryIncludingDeleted(x => x.IsDeleted)
                .OrderBy(x => x.Name)
                .Select(x => new CategoryViewModel
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .ToListAsync();
        }
    }
}