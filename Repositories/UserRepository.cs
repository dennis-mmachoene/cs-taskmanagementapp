using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskManagementApp.Data;
using TaskManagementApp.Entities;

namespace TaskManagementApp.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext appDbContext;
        private readonly UserManager<User> userManager;

        public UserRepository(AppDbContext appDbContext, UserManager<User> userManager)
        {
            this.appDbContext = appDbContext;
            this.userManager = userManager;
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await appDbContext.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<bool> ExistsAsync(string id)
        {
            return await appDbContext.Users.AnyAsync(u => u.Id == id);
        }

        public async Task<int> GetActiveUsersCountAsync(int daysThreshold = 30)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-daysThreshold);
            return await appDbContext.Users.CountAsync(u =>
                u.LastLoginAt.HasValue && u.LastLoginAt >= cutoffDate
            );
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await appDbContext
                .Users.OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .ToListAsync();
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await appDbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetByIdAsync(string id)
        {
            return await appDbContext.Users.FindAsync(id);
        }

        public async Task<int> GetTotalUsersCountAsync()
        {
            return await appDbContext.Users.CountAsync();
        }

        public async Task<IEnumerable<User>> GetUsersInRoleAsync(string roleName)
        {
            return await userManager.GetUsersInRoleAsync(roleName);
        }

        public async Task<User?> GetUserWithTasksAsync(string userId)
        {
            return await appDbContext
                .Users.Include(u => u.Tasks.OrderByDescending(t => t.CreatedAt))
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<IEnumerable<User>> SearchUserAsync(string searchItem)
        {
            if (string.IsNullOrEmpty(searchItem))
            {
                return await GetAllAsync();
            }

            var lowerSearchItem = searchItem.ToLower();

            return await appDbContext
                .Users.Where(u =>
                    u.FirstName.ToLower().Contains(lowerSearchItem)
                    || u.LastName.ToLower().Contains(lowerSearchItem)
                    || u.Email!.ToLower().Contains(lowerSearchItem)
                )
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .ToListAsync();
        }

        public async Task<bool> UpdateLastLoginAsync(string userId)
        {
            var user = await appDbContext.Users.FindAsync(userId);
            if (user == null)
            {
                return false;
            }
            user.LastLoginAt = DateTime.UtcNow;

            try
            {
                await appDbContext.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateUserProfileAsync(User user)
        {
            try
            {
                appDbContext.Users.Update(user);
                await appDbContext.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
