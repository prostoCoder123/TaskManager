using Entities;
using TaskManager.Dto;

namespace TaskManager.Abstractions;

public interface ITaskService
{
    Task<TasksPageDto> GetTasksAsync(
        (int pageNumber, int elementsOnPage) paging,
        ProjectTaskStatus? status,
        CancellationToken ct = default);

    Task<(ProjectTask? createdTask, IEnumerable<string> errors)> AddTaskAsync(
        ProjectTask taskToAdd,
        CancellationToken ct = default);

    Task<(ProjectTask? updatedTask, IEnumerable<string> errors)> UpdateTaskAsync(
        ProjectTask taskToUpdate,
        UpdateTaskDto taskToMapFrom,
        CancellationToken ct = default);

    Task<(IEnumerable<ProjectTask>?, IEnumerable<string>)> FixOverDueTasksAsync(CancellationToken ct = default);
}
