using System.ComponentModel.DataAnnotations;

namespace Demo.ViewModels
{
    public class InstructorViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;

        [Phone]
        [StringLength(20)]
        public string? Phone { get; set; }

        [StringLength(500)]
        public string? Bio { get; set; }
    }
}