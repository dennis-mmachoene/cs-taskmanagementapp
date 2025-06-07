using TaskManagementApp.Entities;

namespace TaskManagementApp.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByIdAsync(string id);
        Task<User?> GetByEmailAsync(string email);
        Task<IEnumerable<User>> GetUsersInRoleAsync(string roleName);

        Task<int> GetTotalUsersCountAsync();
        Task<int> GetActiveUsersCountAsync(int daysThreshold = 30);

        Task<User?> GetUserWithTasksAsync(string userId);

        Task<IEnumerable<User>> SearchUserAsync(string searchItem);

        Task<bool> UpdateLastLoginAsync(string userId);
        Task<bool> UpdateUserProfileAsync(User user);

        Task<bool> ExistsAsync(string id);
        Task<bool> EmailExistsAsync(string email);
    }
}