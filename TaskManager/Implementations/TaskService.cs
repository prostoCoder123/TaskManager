using Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TaskManager.Abstractions;
using TaskManager.Dto;

namespace TaskManager.Implementations;

public class TaskService(ITaskRepository taskRepository) : ITaskService
{
    private static Expression<Func<ProjectTask, bool>> FilterByStatus(
        ProjectTaskStatus? status = null
    ) => t => status == null || t.Status == status;

    public Task<TasksPageDto> GetTasksAsync(
        (int pageNumber, int elementsOnPage) paging,
        ProjectTaskStatus? status,
        CancellationToken ct = default)
    {
        var filter = FilterByStatus(status);

        int filteredCount = taskRepository.Count(filter.Compile());

        var tasks = (taskRepository.Get(
            _filter: filter,
            _paging: paging,
            _orderBy: _q => _q.OrderByDescending(c => c.CreatedAt)
        )).AsAsyncEnumerable();

        return Task.FromResult(
            new TasksPageDto()
            {
                Tasks = tasks,
                Total = filteredCount,
                PageNumber = paging.pageNumber
            }
        );
    }
}
