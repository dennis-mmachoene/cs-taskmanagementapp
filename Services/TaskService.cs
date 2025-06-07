using TaskManagementApp.Entities;
using TaskManagementApp.Repositories;

namespace TaskManagementApp.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository taskRepository;
        private readonly IUserRepository userRepository;

        public TaskService(ITaskRepository taskRepository, IUserRepository userRepository)
        {
            this.taskRepository = taskRepository;
            this.userRepository = userRepository;
        }

        public async Task<bool> CanUserAccessTaskAsync(int taskId, string userId)
        {
            return await taskRepository.ExistsForUserAsync(taskId, userId);
        }

        public async Task<(bool Success, TaskItem? Task, string ErrorMessage)> CreateTaskAsync(
            TaskItem task,
            string userId
        )
        {
            if (!await userRepository.ExistsAsync(userId))
            {
                return (false, null, "User not found");
            }
            task.UserId = userId;
            task.CreatedAt = DateTime.UtcNow;
            task.CompletedAt = null;

            if (!await ValidateTaskDataAsync(task))
            {
                return (false, null, "Provide the correct details of the task");
            }

            try
            {
                var createdTask = await taskRepository.CreateAsync(task);
                return (true, createdTask, string.Empty);
            }
            catch (Exception ex)
            {
                return (false, null, $"Failed to create taks: {ex.Message}");
            }
        }

        public async Task<(bool Success, string ErrorMessage)> DeleteTaskAsync(
            int id,
            string userId,
            bool isAdmin = false
        )
        {
            if (!await CanUserAccessTaskAsync(id, userId) && !isAdmin)
            {
                return (false, "Task not found or access denied");
            }

            try
            {
                bool deleted = isAdmin
                    ? await taskRepository.DeleteAsync(id)
                    : await taskRepository.DeleteByIdAndUserIdAsync(id, userId);

                if (!deleted)
                {
                    return (false, "Task not found");
                }

                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                return (false, $"Failed to delete task: {ex.Message}");
            }
        }

        public async Task<IEnumerable<TaskItem>> GetAllTasksAsync()
        {
            return await taskRepository.GetAllAsync();
        }

        public async Task<TaskStatistics> GetSystemTaskStatisticsAsync()
        {
            var tasks = await taskRepository.GetAllAsync();
            return CalculateStatistics(tasks);
        }

        public async Task<TaskItem?> GetTaskByIdAsync(int id, string userId, bool isAdmin = false)
        {
            if (isAdmin)
            {
                return await taskRepository.GetByIdAsync(id);
            }

            return await taskRepository.GetByIdAndUserIdAsync(id, userId);
        }

        public async Task<IEnumerable<TaskItem>> GetTasksByPriorityAsync(
            TaskPriority priority,
            string userId,
            bool isAdmin = false
        )
        {
            var allTasks = await taskRepository.GetAllAsync();
            if (isAdmin)
            {
                return allTasks;
            }

            return allTasks.Where(t => t.UserId == userId);
        }

        public async Task<IEnumerable<TaskItem>> GetTasksByStatusAsync(
            Entities.TaskStatus status,
            string userId,
            bool isAdmin = false
        )
        {
            var allTasks = await taskRepository.GetByStatusAsync(status);
            if (isAdmin)
            {
                return allTasks;
            }

            return allTasks.Where(t => t.UserId == userId);
        }

        public async Task<IEnumerable<TaskItem>> GetUserTaskAsync(string userId)
        {
            return await taskRepository.GetByUserIdAsync(userId);
        }

        public async Task<TaskStatistics> GetUserTasksStatisticsAsync(string userId)
        {
            var tasks = await taskRepository.GetByUserIdAsync(userId);
            return CalculateStatistics(tasks);
        }

        public async Task<(bool Success, string ErrorMessage)> MarkTaskCompletedAsync(
            int id,
            string userId
        )
        {
            var task = await taskRepository.GetByIdAndUserIdAsync(id, userId);
            if (task == null)
            {
                return (false, "Task not found");
            }

            task.Status = Entities.TaskStatus.Completed;
            task.CompletedAt = DateTime.UtcNow;

            var results = await UpdateTaskAsync(task, userId);
            return (results.Success, results.ErrorMessage);
        }

        public async Task<(bool Success, string ErrorMessage)> MarkTaskInProgressAsync(
            int id,
            string userId
        )
        {
            var task = await taskRepository.GetByIdAndUserIdAsync(id, userId);
            if (task == null)
            {
                return (false, "Task not found");
            }

            task.Status = Entities.TaskStatus.InProgress;
            task.CompletedAt = null;

            var results = await UpdateTaskAsync(task, userId);
            return (results.Success, results.ErrorMessage);
        }

        public async Task<IEnumerable<TaskItem>> SearchTasksAsync(
            string searchItem,
            string userId,
            bool isAdmin = false
        )
        {
            return await taskRepository.SearchAsync(searchItem, isAdmin ? null : userId);
        }

        public async Task<(bool Success, TaskItem? Task, string ErrorMessage)> UpdateTaskAsync(
            TaskItem task,
            string userId,
            bool isAdmin = false
        )
        {
            var existingTask = await GetTaskByIdAsync(task.Id, userId, isAdmin);
            if (existingTask == null)
            {
                return (false, null, "Task not found or access denied");
            }

            task.CreatedAt = existingTask.CreatedAt;
            task.UserId = existingTask.UserId;

            if (
                task.Status == Entities.TaskStatus.Completed
                && existingTask.Status != Entities.TaskStatus.Completed
            )
            {
                task.CompletedAt = DateTime.UtcNow;
            }
            else
            {
                task.CompletedAt = null;
            }

            if (!await ValidateTaskDataAsync(task))
            {
                return (false, null, "Provide the correct details of the task");
            }

            try
            {
                var updatedTask = await taskRepository.UpdateAsync(task);
                return (true, updatedTask, string.Empty);
            }
            catch (Exception ex)
            {
                return (false, null, $"An error occured while updating task: {ex.Message}");
            }
        }

        public async Task<bool> ValidateTaskDataAsync(TaskItem task)
        {
            if (string.IsNullOrEmpty(task.Title))
            {
                return false;
            }

            if (task.Title.Length > 200)
            {
                return false;
            }

            if (!string.IsNullOrEmpty(task.Description) && task.Description.Length > 1000)
            {
                return false;
            }

            if (task.DueDate.HasValue && task.DueDate < DateTime.UtcNow.Date)
            {
                return false;
            }
            return true;
        }

        private static TaskStatistics CalculateStatistics(IEnumerable<TaskItem> tasks)
        {
            var taskList = tasks.ToList();
            var now = DateTime.UtcNow;

            return new TaskStatistics
            {
                TotalTasks = taskList.Count,
                CompletedTasks = taskList.Count(t => t.Status == Entities.TaskStatus.Completed),
                InProgressTasks = taskList.Count(t => t.Status == Entities.TaskStatus.InProgress),
                PendingTasks = taskList.Count(t => t.Status == Entities.TaskStatus.Pending),
                CancelledTasks = taskList.Count(t => t.Status == Entities.TaskStatus.Cancelled),
                OverdueTasks = taskList.Count(t =>
                    t.DueDate.HasValue
                    && t.DueDate < now
                    && t.Status != Entities.TaskStatus.Completed
                ),
                HighPriorityTasks = taskList.Count(t =>
                    t.Priority == TaskPriority.High || t.Priority == TaskPriority.Critical
                ),
            };
        }
    }
}
