using TaskManagementApp.Entities;

namespace TaskManagementApp.Repositories
{
    public interface ITaskRepository
    {
        Task<IEnumerable<TaskItem>> GetAllAsync();
        Task<IEnumerable<TaskItem>> GetByUserIdAsync(string userId);
        Task<TaskItem?> GetByIdAsync(int id);
        Task<TaskItem?> GetByIdAndUserIdAsync(int id, string userId);

        Task<IEnumerable<TaskItem>> GetByStatusAsync(Entities.TaskStatus status);
        Task<IEnumerable<TaskItem>> GetByPriorityAsync(TaskPriority priority);
        Task<IEnumerable<TaskItem>> SearchAsync(string searchItem, string? userId = null);

        Task<int> GetTaskCountByUserAsync(string userId);
        Task<int> GetCompletedTaskCountByUserAsync(string userId);

        Task<TaskItem> CreateAsync(TaskItem task);
        Task<TaskItem> UpdateAsync(TaskItem task);
        Task<bool> DeleteAsync(int id);
        Task<bool> DeleteByIdAndUserIdAsync(int id, string userId);

        Task<bool> ExistsAsync(int id);
        Task<bool> ExistsForUserAsync(int id, string userId);
    }
}
