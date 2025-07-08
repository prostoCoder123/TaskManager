using Entities;
using TaskManager.Abstractions;
using TaskManager.EfCore;

namespace TaskManager.Implementations;

public class TaskRepository(ProjectTaskContext context) : GenericRepository<ProjectTask>(context), ITaskRepository, IDisposable
{
    private bool disposed = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                context.Dispose();
            }
        }
        disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}