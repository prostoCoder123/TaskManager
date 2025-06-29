using Entities;
using TaskManager.Dto;

namespace TaskManager.Abstractions;

public interface ITaskService
{
    Task<TasksPageDto> GetTasksAsync(
        (int pageNumber, int elementsOnPage) paging,
        ProjectTaskStatus? status,
        CancellationToken ct = default);
}
