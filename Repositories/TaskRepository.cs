using Microsoft.EntityFrameworkCore;
using TaskManagementApp.Data;
using TaskManagementApp.Entities;

namespace TaskManagementApp.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly AppDbContext appDbContext;

        public TaskRepository(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public async Task<TaskItem> CreateAsync(TaskItem task)
        {
            await appDbContext.TaskItems.AddAsync(task);
            await appDbContext.SaveChangesAsync();

            return await GetByIdAsync(task.Id) ?? task;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var task = await appDbContext.TaskItems.FindAsync(id);

            if (task == null)
            {
                return false;
            }

            appDbContext.Remove(task);
            await appDbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteByIdAndUserIdAsync(int id, string userId)
        {
            var task = await appDbContext.TaskItems.FirstOrDefaultAsync(t =>
                t.Id == id && t.UserId == userId
            );
            if (task == null)
            {
                return false;
            }

            appDbContext.TaskItems.Remove(task);
            await appDbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await appDbContext.TaskItems.AnyAsync(t => t.Id == id);
        }

        public async Task<bool> ExistsForUserAsync(int id, string userId)
        {
            return await appDbContext.TaskItems.AnyAsync(t => t.Id == id && t.UserId == userId);
        }

        public async Task<IEnumerable<TaskItem>> GetAllAsync()
        {
            return await appDbContext
                .TaskItems.Include(t => t.User)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<TaskItem?> GetByIdAndUserIdAsync(int id, string userId)
        {
            return await appDbContext.TaskItems.FirstOrDefaultAsync(t =>
                t.Id == id && t.UserId == userId
            );
        }

        public async Task<TaskItem?> GetByIdAsync(int id)
        {
            return await appDbContext
                .TaskItems.Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<TaskItem>> GetByPriorityAsync(TaskPriority priority)
        {
            return await appDbContext
                .TaskItems.Include(t => t.User)
                .Where(t => t.Priority == priority)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<TaskItem>> GetByStatusAsync(Entities.TaskStatus status)
        {
            return await appDbContext
                .TaskItems.Include(t => t.User)
                .Where(t => t.Status == status)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<TaskItem>> GetByUserIdAsync(string userId)
        {
            return await appDbContext
                .TaskItems.Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<int> GetCompletedTaskCountByUserAsync(string userId)
        {
            return await appDbContext.TaskItems.CountAsync(t =>
                t.UserId == userId && t.Status == Entities.TaskStatus.Completed
            );
        }

        public async Task<int> GetTaskCountByUserAsync(string userId)
        {
            return await appDbContext.TaskItems.CountAsync(t => t.UserId == userId);
        }

        public async Task<IEnumerable<TaskItem>> SearchAsync(
            string searchItem,
            string? userId = null
        )
        {
            var query = appDbContext.TaskItems.Include(t => t.User).AsQueryable();

            if (!string.IsNullOrEmpty(userId))
            {
                query = query.Where(t => t.UserId == userId);
            }

            if (!string.IsNullOrEmpty(searchItem))
            {
                query = query.Where(t =>
                    t.Title.Contains(searchItem)
                    || (t.Description != null && t.Description.Contains(searchItem))
                );
            }

            return await query.OrderByDescending(t => t.CreatedAt).ToListAsync();
        }

        public async Task<TaskItem> UpdateAsync(TaskItem task)
        {
            appDbContext.TaskItems.Update(task);
            await appDbContext.SaveChangesAsync();

            return await GetByIdAsync(task.Id) ?? task;
        }
    }
}
