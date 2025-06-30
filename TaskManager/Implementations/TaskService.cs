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

        var tasks = (taskRepository.Get(
            filter: filter,
            paging: paging,
            orderBy: _q => _q.OrderByDescending(c => c.CreatedAt)
        )).AsAsyncEnumerable();

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

        var strategy = context.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await unitOfWork.BeginTransactionAsync();
            try
            {
                await taskRepository.InsertAsync(taskToAdd);
                await unitOfWork.SaveAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                if (ex is DbUpdateException) //TODO: fix autoincrement
                {
                    if ((ex.GetBaseException() as NpgsqlException)?.SqlState == "23505") // Data duplication
                    {
                        errors = [ "Duplicated task names are not allowed" ];
                    }
                }
                else
                {
                    throw;
                }
            }
        });

        return (taskToAdd, errors);
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

        if (taskToAdd.DueDate <= DateTime.UtcNow) // TODO: minimal difference from now
        {
            errors.Add("Task due date must be a future date");
        }

        return errors;
    }
}
