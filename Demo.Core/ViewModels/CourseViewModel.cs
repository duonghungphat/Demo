using Demo.Models;
using System.ComponentModel.DataAnnotations;

namespace Demo.ViewModels
{
    public class CourseViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        public CourseLevel Level { get; set; }

        public CourseStatus Status { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        [Range(1, int.MaxValue,
            ErrorMessage = "Please select a course category.")]
        public int CourseCategoryId { get; set; }

        [Range(1, int.MaxValue,
            ErrorMessage = "Please select an instructor.")]
        public int InstructorId { get; set; }

        [Range(0, 100,
            ErrorMessage = "Discount percent must be between 0 and 100.")]
        public decimal DiscountPercent { get; set; }

        public string? ImagePath { get; set; }

        public string? CourseCategoryName { get; set; }

        public string? InstructorName { get; set; }
        public decimal FinalPrice { get; set; }

        public List<CourseCategoryViewModel> CourseCategories { get; set; } = new();

        public List<InstructorViewModel> Instructors { get; set; } = new();
        public List<LessonViewModel> Lessons { get; set; } = new();
    }
}