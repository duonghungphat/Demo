using System.ComponentModel.DataAnnotations;

namespace Demo.ViewModels
{
    public class ExpenseViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Note is required.")]
        [StringLength(200)]
        public string Note { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue,
            ErrorMessage = "Amount must be greater than zero.")]
        public int Amount { get; set; }

        [Required(ErrorMessage = "Expense date is required.")]
        [DataType(DataType.Date)]
        public DateTime? ExpenseDate { get; set; }

        [Range(1, int.MaxValue,
            ErrorMessage = "Please select a category.")]
        public int CategoryId { get; set; }

        public string? CategoryName { get; set; }

        public List<CategoryViewModel> Categories { get; set; } = new();
    }
}