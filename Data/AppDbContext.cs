using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskManagementApp.Entities;

namespace TaskManagementApp.Data
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions options)
            : base(options) { }

        public DbSet<TaskItem> TaskItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TaskItem>(entity =>
            {
                entity.ToTable("TaskItems");

                entity.HasKey(t => t.Id);

                entity.Property(t => t.Title).HasMaxLength(200).IsRequired();

                entity.Property(t => t.Description).HasMaxLength(1000).IsRequired(false);

                entity
                    .HasOne(t => t.User)
                    .WithMany(u => u.Tasks)
                    .HasForeignKey(t => t.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(t => t.UserId).HasDatabaseName("IX_TaskItems_UserId");

                entity.HasIndex(t => t.Status).HasDatabaseName("IX_TaskItems_Status");

                entity.HasIndex(t => t.CreatedAt).HasDatabaseName("IX_TaskItems_CreatedAt");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(u => u.FirstName).HasMaxLength(50).IsRequired();
                entity.Property(u => u.LastName).HasMaxLength(50).IsRequired();
            });
        }
    }
}
