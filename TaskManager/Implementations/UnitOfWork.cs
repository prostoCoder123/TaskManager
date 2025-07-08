using Microsoft.EntityFrameworkCore.Storage;
using TaskManager.Abstractions;
using TaskManager.EfCore;

namespace TaskManager.Implementations;

public class UnitOfWork(
  ProjectTaskContext context,
  ITaskRepository taskRepository) : IUnitOfWork, IDisposable
{
    public ITaskRepository TaskRepository { get; } = taskRepository;

    //public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken _ct = default) => dbContext.Database.BeginTransactionAsync(_ct);
    public IExecutionStrategy CreateExecutionStrategy() => dbContext.Database.CreateExecutionStrategy();

    public async Task SaveAsync(CancellationToken _ct = default) => _ = await dbContext.SaveChangesAsync(_ct);

    private readonly ProjectTaskContext dbContext = context;

    private bool disposed;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// the Unit of Work should implement IDisposable and dispose the context, not the repositories.
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                dbContext.Dispose();
            }
        }

        disposed = true;
    }
}