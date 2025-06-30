using Microsoft.EntityFrameworkCore.Storage;

namespace TaskManager.Abstractions;

public interface IUnitOfWork
{
    ITaskRepository TaskRepository { get; }
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken ct = default);
    Task SaveAsync(CancellationToken ct = default);
    TEntity DetachedClone<TEntity>(TEntity entity) where TEntity : class;
}