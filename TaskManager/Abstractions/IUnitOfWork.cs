using Microsoft.EntityFrameworkCore.Storage;

namespace TaskManager.Abstractions;

public interface IUnitOfWork
{
  ITaskRepository TaskRepository { get; }
  Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken _ct = default);
  Task SaveAsync(CancellationToken _ct = default);
  TEntity DetachedClone<TEntity>(TEntity _entity) where TEntity : class;
}