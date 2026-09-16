using System.ComponentModel.DataAnnotations;

namespace Demo.ViewModels
{
    public class CourseCategoryViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(300)]
        public string Description { get; set; } = string.Empty;
    }
}