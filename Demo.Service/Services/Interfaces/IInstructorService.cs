using Demo.ViewModels;

namespace Demo.Service.Services.Interfaces
{
    public interface IInstructorService
    {
        Task<List<InstructorViewModel>> GetAllAsync();
        Task<InstructorViewModel?> GetByIdAsync(int id);

        Task CreateAsync(InstructorViewModel instructor);
        Task UpdateAsync(InstructorViewModel instructor);
        Task DeleteAsync(int id);
    }
}