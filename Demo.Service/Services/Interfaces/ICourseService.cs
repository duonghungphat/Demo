using Demo.ViewModels;

namespace Demo.Service.Services.Interfaces
{
    public interface ICourseService
    {
        Task<List<CourseViewModel>> GetAllAsync(string? keyword = null);
        Task<CourseViewModel?> GetByIdAsync(int id);

        Task CreateAsync(CourseViewModel course);
        Task UpdateAsync(CourseViewModel course);
        Task DeleteAsync(int id);

        Task PublishAsync(int id);
        Task CloseAsync(int id);

        Task<LessonViewModel?> GetLessonByIdAsync(int lessonId);
        Task AddLessonAsync(LessonViewModel lesson);
        Task UpdateLessonAsync(LessonViewModel lesson);
        Task DeleteLessonAsync(int lessonId);
        decimal CalculateFinalPrice(CourseViewModel course);
    }
}