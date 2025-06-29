using Entities;
using Microsoft.EntityFrameworkCore;

namespace TaskManager.EfCore;

public class ProjectTaskContext : DbContext
{
    public ProjectTaskContext(DbContextOptions<ProjectTaskContext> options)
        : base(options)
    {
    }

    public DbSet<ProjectTask> ProjectTasks { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        _ = modelBuilder.HasPostgresEnum<ProjectTaskStatus>(name: "task_status");

        _ = modelBuilder.Entity<ProjectTask>().ToTable("project_tasks")
            .HasKey(b => b.Id)
            .HasName("tasks_pkey");

        _ = modelBuilder.Entity<ProjectTask>().Property(p => p.Id).ValueGeneratedOnAdd();
        _ = modelBuilder.Entity<ProjectTask>().Property(p => p.Title).HasMaxLength(Constants.MaxLength100).IsRequired(true);
        _ = modelBuilder.Entity<ProjectTask>().Property(p => p.Description).HasMaxLength(Constants.MaxLength1000).IsRequired(true);
    }
}
