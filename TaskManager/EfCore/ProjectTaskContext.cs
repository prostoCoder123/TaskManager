using Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

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
        modelBuilder.HasPostgresEnum<ProjectTaskStatus>(name: "task_status");

        modelBuilder.Entity<ProjectTask>().ToTable("project_tasks")
            .HasKey(b => b.Id)
            .HasName("tasks_pkey");

        modelBuilder.Entity<ProjectTask>().Property(p => p.Id).ValueGeneratedOnAdd();

        modelBuilder.Entity<ProjectTask>().Property(p => p.Title).HasMaxLength(Constants.MaxLength100).IsRequired(true);
        modelBuilder.Entity<ProjectTask>().Property(p => p.Description).HasMaxLength(Constants.MaxLength1000).IsRequired(true);

        modelBuilder.Entity<ProjectTask>().HasIndex(i => i.Title).IsUnique();
    }
}