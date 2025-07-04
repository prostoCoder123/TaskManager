﻿using AutoMapper;
using Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Linq.Expressions;
using TaskManager.Abstractions;
using TaskManager.Dto;
using TaskManager.EfCore;

namespace TaskManager.Implementations;

public class TaskService(
    ITaskRepository taskRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ProjectTaskContext context) : ITaskService
{
    private static Expression<Func<ProjectTask, bool>> FilterByStatus(
        ProjectTaskStatus? status = null
    ) => t => status == null || t.Status == status;

    public async Task<TasksPageDto> GetTasksAsync(
        (int pageNumber, int elementsOnPage) paging,
        ProjectTaskStatus? status,
        CancellationToken ct = default)
    {
        var filter = FilterByStatus(status);

        int filteredCount = await taskRepository.CountAsync(filter);

        var tasks = taskRepository.Get(
            filter: filter,
            paging: paging,
            orderBy: q => q.OrderByDescending(c => c.CreatedAt)
        )
        .AsNoTracking()
        .AsAsyncEnumerable();

        return new TasksPageDto()
        {
            Tasks = tasks,
            Total = filteredCount,
            PageNumber = paging.pageNumber
        };
    }

    public async Task<(ProjectTask?, IEnumerable<string>)> AddTaskAsync(
        ProjectTask taskToAdd,
        CancellationToken ct = default)
    {
        var errors = ValidateTaskToAdd(taskToAdd);

        if (errors.Any())
        {
            return (null, errors);
        }

        return await ExecuteTransactionAsync(
            taskToAdd,
            async (t, ct) => await taskRepository.InsertAsync(t, ct),
            ct);
    }

    public async Task<(ProjectTask? updatedTask, IEnumerable<string> errors)> UpdateTaskAsync(
        ProjectTask taskToUpdate,
        UpdateTaskDto taskToMapFrom,
        CancellationToken ct = default)
    {
        var errors = ValidateTaskToUpdate(taskToUpdate, taskToMapFrom);

        if (errors.Any())
        {
            return (null, errors);
        }

        return await ExecuteTransactionAsync(taskToUpdate, (t, ct) =>
        {
            mapper.Map(taskToMapFrom, t);
            taskRepository.Update(t);
        }, ct);
    }

    public async Task<(IEnumerable<ProjectTask>?, IEnumerable<string>)> FixOverDueTasksAsync(CancellationToken ct = default)
    {
        // find all the tasks that are not completed at the specified time
        IEnumerable<ProjectTask> tasksToFix = taskRepository.Get(
            filter: t => t.Status != ProjectTaskStatus.Completed &&
                         t.Status != ProjectTaskStatus.OverDue &&
                         t.CompletedAt == null &&
                         t.DueDate <= DateTime.UtcNow, // compare in utc
            orderBy: t => t.OrderBy(x => x.Id))
        .Take(Constants.MaxElementsOnPage) // TODO: handle all by portions
        .ToArray();

        return tasksToFix.Any()
            ? await ExecuteTransactionAsync(
                tasksToFix, (t, ct) =>
                {
                    foreach (ProjectTask task in tasksToFix)
                    {
                        task.Status = ProjectTaskStatus.OverDue;
                        taskRepository.Update(task);
                    }
                }, ct)
            : (null, []);
    }

    public async Task<ProjectTask?> GetTaskByIdAsync(int taskId, CancellationToken ct = default) =>
        await taskRepository.GetByIdAsync(taskId, ct);

    private async Task<(T? updated, IEnumerable<string> errors)> ExecuteTransactionAsync<T>(
         T entity,
         Action<T, CancellationToken> operation,
         CancellationToken ct = default)
    {
        var errors = new List<string>();

        var strategy = context.Database.CreateExecutionStrategy(); // TODO: ResilientTransaction
        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await unitOfWork.BeginTransactionAsync(ct);
            try
            {
                operation.Invoke(entity, ct);
                await unitOfWork.SaveAsync(ct);
                await transaction.CommitAsync(ct);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(ct);

                if (ex is DbUpdateException) //TODO: fix autoincrement
                {
                    if ((ex.GetBaseException() as NpgsqlException)?.SqlState == "23505") // Data duplication
                    {
                        errors = ["Duplicated task names are not allowed"];
                    }
                }
                else
                {
                    throw;
                }
            }
        });

        return (entity, errors);
    }

    private IEnumerable<string> ValidateTaskToAdd(ProjectTask taskToAdd)
    {
        ICollection<string> errors = new List<string>();

        if (string.IsNullOrWhiteSpace(taskToAdd.Title))
        {
            errors.Add("Task name can not be empty");
        }

        if (string.IsNullOrWhiteSpace(taskToAdd.Description))
        {
            errors.Add("Task description can not be empty");
        }

        if (taskToAdd.Title.Length > Constants.MaxLength100)
        {
            errors.Add($"Task title can not be more than {Constants.MaxLength100} characters");
        }

        if (taskToAdd.Description.Length > Constants.MaxLength1000)
        {
            errors.Add($"Task description can not be more than {Constants.MaxLength1000} characters");
        }

        if (taskToAdd.DueDate <= DateTime.UtcNow.AddMinutes(1)) // TODO: minimal difference from now
        {
            errors.Add("Task due date must be a future date");
        }

        return errors;
    }

    private IEnumerable<string> ValidateTaskToUpdate(ProjectTask taskToUpdate, UpdateTaskDto taskToMapFrom)
    {
        if (taskToUpdate.Status == ProjectTaskStatus.OverDue)
        {
            return ["Can not modify a task with the 'OverDue' status"];
        }

        if ((taskToMapFrom.Title == null || taskToUpdate.Title == taskToMapFrom.Title) &&
            (taskToMapFrom.Description == null || taskToUpdate.Description == taskToMapFrom.Description) &&
            (taskToMapFrom.Status == null || taskToUpdate.Status == taskToMapFrom.Status) &&
            (taskToMapFrom.DueDate == null || taskToUpdate.DueDate == taskToMapFrom.DueDate))
        {
            return ["Nothing has changed"];
        }

        ICollection<string> errors = ValidateTaskToAdd(taskToUpdate).ToList();

        /*if (taskToMapFrom.Status < taskToUpdate.Status)
        {
            errors.Add("Task status can not be downgraded");
        }*/

        return errors;
    }
}