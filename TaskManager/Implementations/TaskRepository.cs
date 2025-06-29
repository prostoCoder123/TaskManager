using Entities;
using TaskManager.Abstractions;
using TaskManager.EfCore;

namespace TaskManager.Implementations;

public class TaskRepository(ProjectTaskContext _context) : GenericRepository<ProjectTask>(_context), ITaskRepository
{
}
