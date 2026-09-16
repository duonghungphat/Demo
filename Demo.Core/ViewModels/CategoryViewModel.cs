using System.ComponentModel.DataAnnotations;

namespace Demo.ViewModels
{
    public class CategoryViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Category name is required.")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
    }
}