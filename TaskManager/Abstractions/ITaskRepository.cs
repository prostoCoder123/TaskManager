using Entities;

namespace TaskManager.Abstractions;

/// <summary>
/// Repository for domain model (not for a table).
/// Since domain model may contain aggregated data between couple tables.
/// </summary>
public interface ITaskRepository : IGenericRepository<ProjectTask>, IDisposable
{
}