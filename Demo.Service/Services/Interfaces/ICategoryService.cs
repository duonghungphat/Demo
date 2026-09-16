using Demo.ViewModels;

namespace Demo.Service.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<List<CategoryViewModel>> GetAllAsync();
        Task<CategoryViewModel?> GetByIdAsync(int id);

        Task CreateAsync(CategoryViewModel category);
        Task UpdateAsync(CategoryViewModel category);
        Task DeleteAsync(int id);
        Task<List<CategoryViewModel>> GetDeletedAsync();
    }
}