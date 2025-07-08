using Microsoft.EntityFrameworkCore.Storage;

namespace TaskManager.Abstractions;

/// <summary>
/// The unit of work class serves one purpose:
/// to make sure that when you use multiple repositories, 
/// they share a single database context (and thus coordinate all updates).
/// ***
/// Repositories will handle querying and crud operations,
/// then unit of work will handle the commit and rollback (if commit fails).
/// </summary>
public interface IUnitOfWork : IDisposable
{
    ITaskRepository TaskRepository { get; }
    //Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken ct = default);
    IExecutionStrategy CreateExecutionStrategy();
    Task SaveAsync(CancellationToken ct = default);
}