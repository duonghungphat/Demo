namespace Demo.Models
{
    public class Instructor : BaseEntity
    {
        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? Phone { get; set; }

        public string? Bio { get; set; }

        public ICollection<Course> Courses { get; set; }
            = new List<Course>();
    }
}