namespace Demo.Models
{
    public class Lesson : BaseEntity
    {
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public int Order { get; set; }

        public int DurationMinutes { get; set; }

        public int CourseId { get; set; }

        public Course? Course { get; set; }
    }
}