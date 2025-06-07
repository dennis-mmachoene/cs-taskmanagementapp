using TaskManagementApp.Entities;

namespace TaskManagementApp.Services
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskItem>> GetUserTaskAsync(string userId);
        Task<IEnumerable<TaskItem>> GetAllTasksAsync();
        Task<TaskItem?> GetTaskByIdAsync(int id, string userId, bool isAdmin = false);

        //Task Management operations
        Task<(bool Success, TaskItem? Task, string ErrorMessage)> CreateTaskAsync(
            TaskItem taskItem,
            string userId
        );
        Task<(bool Success, TaskItem? Task, string ErrorMessage)> UpdateTaskAsync(
            TaskItem taskItem,
            string userId,
            bool isAdmin = false
        );
        Task<(bool Success, string ErrorMessage)> DeleteTaskAsync(
            int id,
            string userId,
            bool isAdmin = false
        );

        //Status management
        Task<(bool Success, string ErrorMessage)> MarkTaskCompletedAsync(int id, string userId);
        Task<(bool Success, string ErrorMessage)> MarkTaskInProgressAsync(int id, string userId);

        //Searching and Filtering
        Task<IEnumerable<TaskItem>> SearchTasksAsync(
            string searchItem,
            string userId,
            bool isAdmin = false
        );
        Task<IEnumerable<TaskItem>> GetTasksByStatusAsync(
            Entities.TaskStatus status,
            string userId,
            bool isAdmin = false
        );
        Task<IEnumerable<TaskItem>> GetTasksByPriorityAsync(
            TaskPriority priority,
            string userId,
            bool isAdmin = false
        );

        Task<TaskStatistics> GetUserTasksStatisticsAsync(string userId);
        Task<TaskStatistics> GetSystemTaskStatisticsAsync();

        Task<bool> CanUserAccessTaskAsync(int taskId, string userId);
        Task<bool> ValidateTaskDataAsync(TaskItem task);
    }

    public class TaskStatistics
    {
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int InProgressTasks { get; set; }
        public int PendingTasks { get; set; }
        public int CancelledTasks { get; set; }
        public double CompletedRate =>
            TotalTasks > 0 ? (double)CompletedTasks / TotalTasks * 100 : 0;
        public int OverdueTasks { get; set; }
        public int HighPriorityTasks { get; set; }
    }
}
