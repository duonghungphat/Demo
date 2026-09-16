using System.ComponentModel.DataAnnotations;

namespace Demo.ViewModels
{
    public class LessonViewModel
    {
        public int Id { get; set; }

        public int CourseId { get; set; }

        [Required]
        [StringLength(150)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Range(1, 1000)]
        public int Order { get; set; }

        [Range(1, 1000)]
        public int DurationMinutes { get; set; }
    }
}