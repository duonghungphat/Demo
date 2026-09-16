using Demo.ViewModels;

namespace Demo.Service.Services.Interfaces
{
    public interface ICourseCategoryService
    {
        Task<List<CourseCategoryViewModel>> GetAllAsync();

        Task<CourseCategoryViewModel?> GetByIdAsync(int id);

        Task CreateAsync(CourseCategoryViewModel model);

        Task UpdateAsync(CourseCategoryViewModel model);

        Task DeleteAsync(int id);
    }
}