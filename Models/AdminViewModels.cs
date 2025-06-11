using System.ComponentModel.DataAnnotations;
using TaskManagementApp.Services;

namespace TaskManagementApp.Models
{
    /// <summary>
    /// ViewModel for displaying user information in admin panel
    /// </summary>
    public class UserManagementViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public List<string> Roles { get; set; } = new();
        public int TaskCount { get; set; }
        public bool IsLockedOut { get; set; }
        
        public string FullName => $"{FirstName} {LastName}";
        public string LastLoginDisplay => LastLoginAt?.ToString("MMM dd, yyyy") ?? "Never";
    }

    /// <summary>
    /// ViewModel for editing user roles
    /// </summary>
    public class EditUserRolesViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        
        [Display(Name = "Available Roles")]
        public List<RoleSelectionViewModel> Roles { get; set; } = new();
    }

    /// <summary>
    /// ViewModel for role selection checkboxes
    /// </summary>
    public class RoleSelectionViewModel
    {
        public string RoleName { get; set; } = string.Empty;
        public bool IsSelected { get; set; }
    }

    /// <summary>
    /// ViewModel for admin dashboard statistics
    /// </summary>
    public class AdminDashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int AdminUsers { get; set; }
        public TaskStatistics SystemTaskStatistics { get; set; } = new();
        public List<UserManagementViewModel> RecentUsers { get; set; } = new();
        public List<TaskActivityViewModel> RecentTaskActivity { get; set; } = new();
    }

    /// <summary>
    /// ViewModel for displaying recent task activity
    /// </summary>
    public class TaskActivityViewModel
    {
        public string TaskTitle { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty; // Created, Completed, etc.
        public DateTime Timestamp { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}