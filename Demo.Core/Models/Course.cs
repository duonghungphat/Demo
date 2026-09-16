namespace Demo.Models
{
    public class Course : BaseEntity
    {
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public CourseLevel Level { get; set; }

        public CourseStatus Status { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string? ImagePath { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public decimal DiscountPercent { get; set; }

        public int CourseCategoryId { get; set; }

        public CourseCategory? CourseCategory { get; set; }


        public int InstructorId { get; set; }

        public Instructor? Instructor { get; set; }


        public ICollection<Lesson> Lessons { get; set; }
            = new List<Lesson>();
    }


    public enum CourseLevel
    {
        Beginner,
        Intermediate,
        Advanced
    }


    public enum CourseStatus
    {
        Draft,
        Published,
        Closed
    }
}