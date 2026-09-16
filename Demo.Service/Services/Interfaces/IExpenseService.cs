using Demo.ViewModels;

namespace Demo.Service.Services.Interfaces
{
    public interface IExpenseService
    {
        Task<List<ExpenseViewModel>> GetAllAsync();
        Task<ExpenseViewModel?> GetByIdAsync(int id);

        Task CreateAsync(ExpenseViewModel expense);
        Task UpdateAsync(ExpenseViewModel expense);
        Task DeleteAsync(int id);
    }
}