using Demo.Models;
using Demo.Repositories;
using Demo.Service.Services.Interfaces;
using Demo.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Demo.Service.Services.Implementations
{
    public class ExpenseService : IExpenseService
    {
        private readonly IUnitOfWork _unitOfWork;

        private static readonly DateTime MinimumExpenseDate =
            new DateTime(2000, 1, 1);

        public ExpenseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<ExpenseViewModel>> GetAllAsync()
        {
            return await _unitOfWork.Expenses
                .BuildQuery(x => true)
                .Include(x => x.Category)
                .OrderByDescending(x => x.ExpenseDate)
                .Select(x => new ExpenseViewModel
                {
                    Id = x.Id,
                    Note = x.Note,
                    Amount = x.Amount,
                    ExpenseDate = x.ExpenseDate,
                    CategoryId = x.CategoryId,
                    CategoryName = x.Category != null
                        ? x.Category.Name
                        : null
                })
                .ToListAsync();
        }

        public async Task<ExpenseViewModel?> GetByIdAsync(int id)
        {
            return await _unitOfWork.Expenses
                .BuildQuery(x => x.Id == id)
                .Select(x => new ExpenseViewModel
                {
                    Id = x.Id,
                    Note = x.Note,
                    Amount = x.Amount,
                    ExpenseDate = x.ExpenseDate,
                    CategoryId = x.CategoryId
                })
                .FirstOrDefaultAsync();
        }

        public async Task CreateAsync(ExpenseViewModel model)
        {
            await ValidateExpenseAsync(model);

            var expense = new Expense
            {
                Note = model.Note,
                Amount = model.Amount,
                ExpenseDate = model.ExpenseDate!.Value,
                CategoryId = model.CategoryId
            };

            await _unitOfWork.Expenses.AddAsync(expense);

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateAsync(ExpenseViewModel model)
        {
            await ValidateExpenseAsync(model);

            var existing = await _unitOfWork.Expenses
                .GetByIdAsync(model.Id);

            if (existing == null)
            {
                throw new KeyNotFoundException("Expense not found.");
            }

            existing.Note = model.Note;
            existing.Amount = model.Amount;
            existing.ExpenseDate = model.ExpenseDate!.Value;
            existing.CategoryId = model.CategoryId;

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var expense = await _unitOfWork.Expenses
                .GetByIdAsync(id);

            if (expense == null)
            {
                throw new KeyNotFoundException("Expense not found.");
            }

            _unitOfWork.Expenses.Delete(expense);

            await _unitOfWork.SaveChangesAsync();
        }

        private async Task ValidateExpenseAsync(ExpenseViewModel model)
        {
            if (model.Amount <= 0)
            {
                throw new InvalidOperationException("Amount must be greater than zero.");
            }

            if (!model.ExpenseDate.HasValue)
            {
                throw new InvalidOperationException("Expense date is required.");
            }

            var date = model.ExpenseDate.Value.Date;

            if (date < MinimumExpenseDate)
            {
                throw new InvalidOperationException("Expense date cannot be earlier than 01/01/2000.");
            }

            if (date > DateTime.Today)
            {
                throw new InvalidOperationException("Expense date cannot be in the future.");
            }

            if (!await _unitOfWork.Categories.ExistsAsync(model.CategoryId))
            {
                throw new InvalidOperationException("Selected category does not exist.");
            }
        }
    }
}