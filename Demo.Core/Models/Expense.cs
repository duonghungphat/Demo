namespace Demo.Models
{
    public class Expense : BaseEntity
    {
        public string Note { get; set; } = string.Empty;

        public int Amount { get; set; }

        public DateTime ExpenseDate { get; set; }

        public int CategoryId { get; set; }

        public Category? Category { get; set; }
    }
}



