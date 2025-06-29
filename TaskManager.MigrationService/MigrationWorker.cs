using System.Diagnostics;
using TaskManager.EfCore;
using Microsoft.EntityFrameworkCore;

using OpenTelemetry.Trace;
using Entities;

namespace TaskManager.MigrationService;

public class MigrationWorker(
    IServiceProvider serviceProvider,
    IHostApplicationLifetime hostApplicationLifetime) : BackgroundService
{
    public const string ActivitySourceName = "Migrations";
    private static readonly ActivitySource s_activitySource = new(ActivitySourceName);

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var activity = s_activitySource.StartActivity("Migrating database", ActivityKind.Client);

        try
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ProjectTaskContext>();

            await RunMigrationAsync(dbContext, cancellationToken);
            await SeedDataAsync(dbContext, cancellationToken);
        }
        catch (Exception ex)
        {
            activity?.RecordException(ex);
            throw;
        }

        hostApplicationLifetime.StopApplication();
    }

    private static async Task RunMigrationAsync(ProjectTaskContext dbContext, CancellationToken cancellationToken)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await dbContext.Database.MigrateAsync(cancellationToken);
        });
    }

    private static async Task SeedDataAsync(ProjectTaskContext dbContext, CancellationToken cancellationToken)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            // Seed the database
            await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

            var tasks = new List<ProjectTask>
            {
                new ProjectTask { Id = 1, Title = "Update dependencies", Description = "Reinstall and update NuGet packages", CreatedAt = new(2025, 3, 13, 10, 0, 0, DateTimeKind.Utc), Status = ProjectTaskStatus.Completed, CompletedAt = new(2025, 3, 15, 12, 8, 3, DateTimeKind.Utc), DueDate = new(2025, 3, 15, 15, 0, 0, DateTimeKind.Utc), UpdatedAt = new(2025, 3, 15, 12, 8, 3, DateTimeKind.Utc) },
                new ProjectTask { Id = 2, Title = "Fix bug", Description = "unintentional use of nested loops", CreatedAt = new(2025, 3, 10, 14, 9, 44, DateTimeKind.Utc), Status = ProjectTaskStatus.Completed, CompletedAt = new(2025, 3, 11, 17, 32, 56, DateTimeKind.Utc), DueDate = new(2025, 3, 12, 9, 0, 0, DateTimeKind.Utc), UpdatedAt = new(2025, 3, 11, 17, 32, 56, DateTimeKind.Utc)  },
                new ProjectTask { Id = 3, Title = "Add feature", Description = "Add sorting", CreatedAt = new(2025, 3, 15, 15, 52, 7, DateTimeKind.Utc), Status = ProjectTaskStatus.OverDue, CompletedAt = null, DueDate = new(2025, 3, 17, 11, 0, 0, DateTimeKind.Utc), UpdatedAt = new(2025, 3, 17, 11, 0, 0, DateTimeKind.Utc) },
                new ProjectTask { Id = 4, Title = "Upgrade the version of nuget package", Description = "Upgrade the version of StackExchange.Redis to the latest", CreatedAt = new(2025, 3, 14, 12, 30, 8, DateTimeKind.Utc), Status = ProjectTaskStatus.Completed, CompletedAt = new(2025, 3, 14, 16, 43, 31, DateTimeKind.Utc), DueDate = new(2025, 3, 14, 18, 0, 0, DateTimeKind.Utc), UpdatedAt = new(2025, 3, 14, 16, 43, 31, DateTimeKind.Utc) },
                new ProjectTask { Id = 5, Title = "Add dockerfile", Description = "Create and configure a Dockerfile for .NET; Build a Docker image; Create and run a Docker container.", CreatedAt = new(2025, 3, 16, 8, 24, 22, DateTimeKind.Utc), Status = ProjectTaskStatus.Completed, CompletedAt = new(2025, 3, 17, 11, 2, 26, DateTimeKind.Utc), DueDate = new(2025, 3, 18, 18, 0, 0, DateTimeKind.Utc), UpdatedAt = new(2025, 3, 17, 11, 2, 26, DateTimeKind.Utc) },
                new ProjectTask { Id = 6, Title = "Filter tasks by their statuses", Description = "Implement tasks filtering", CreatedAt = new(2025, 3, 11, 9, 4, 47, DateTimeKind.Utc), Status = ProjectTaskStatus.OverDue, CompletedAt = new(2025, 3, 18, 15, 12, 34, DateTimeKind.Utc), DueDate = new(2025, 3, 15, 10, 0, 0, DateTimeKind.Utc), UpdatedAt = new(2025, 3, 18, 15, 12, 34, DateTimeKind.Utc) },
                new ProjectTask { Id = 7, Title = "Add authorization", Description = "restrict access to ASP.NET Core controllers and actions", CreatedAt = new(2025, 6, 20, 9, 8, 2, DateTimeKind.Utc), Status = ProjectTaskStatus.InProgress, CompletedAt = null, DueDate = new(2025, 6, 29, 18, 0, 0, DateTimeKind.Utc), UpdatedAt = new(2025, 6, 22, 16, 47, 23, DateTimeKind.Utc) },
                new ProjectTask { Id = 8, Title = "Implement caching", Description = "adding the Response Caching Middleware services", CreatedAt = new(2025, 6, 23, 17, 10, 53, DateTimeKind.Utc), Status = ProjectTaskStatus.InProgress, CompletedAt = null, DueDate = new(2025, 6, 30, 18, 0, 0, DateTimeKind.Utc), UpdatedAt = new(2025, 6, 24, 8, 56, 11, DateTimeKind.Utc) },
                new ProjectTask { Id = 9, Title = "Add rate limiting middleware", Description = "configure rate limiting policies", CreatedAt = new(2025, 6, 27, 13, 12, 38, DateTimeKind.Utc), Status = ProjectTaskStatus.New, CompletedAt = null, DueDate = new(2025, 7, 5, 18, 0, 0, DateTimeKind.Utc), UpdatedAt = new(2025, 6, 27, 13, 12, 38, DateTimeKind.Utc) },
                new ProjectTask { Id = 10, Title = "Add logging middleware", Description = "provide logs of HTTP request information", CreatedAt = new(2025, 6, 19, 11, 17, 24, DateTimeKind.Utc), Status = ProjectTaskStatus.InProgress, CompletedAt = null, DueDate = new(2025, 7, 1, 18, 0, 0, DateTimeKind.Utc), UpdatedAt = new(2025, 6, 20, 16, 30, 33, DateTimeKind.Utc) },
            };

            dbContext.Set<ProjectTask>().AddRange(tasks);

            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        });
    }
}
