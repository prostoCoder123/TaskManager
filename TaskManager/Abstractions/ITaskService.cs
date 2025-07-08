using Entities;
using TaskManager.Dto;

namespace TaskManager.Abstractions;

/// <summary>
/// The Service Repository pattern is a software design pattern
/// that separates the concerns of business logic and data persistence in an application.
/// It involves two main layers:
/// the Service layer, which handles business logic and interacts with the Repository,
/// and the Repository layer, which abstracts data access operations.
/// </summary>
public interface ITaskService : IDisposable
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

    Task<ProjectTask?> GetTaskByIdAsync(int taskId, CancellationToken ct = default);
}