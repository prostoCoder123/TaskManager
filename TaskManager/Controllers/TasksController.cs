using AutoMapper;
using Entities;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Abstractions;
using TaskManager.Dto;

namespace TaskManager.Controllers;

[ApiController]
[Route("[controller]")]
public class TasksController(
    ITaskService taskService,
    IMapper mapper,
    ITaskRepository taskRepository,
    ILogger<TasksController> logger) : ControllerBase
{
    /// <summary>
    /// Возвращает страницу с задачами (отсортированные по дате создания по убыванию)
    /// </summary>
    /// <param name="page">данные страницы (номер страницы с нуля, количество сущностей на странице)</param>
    /// <param name="status">статус задач</param>
    /// <param name="ct">токен отмены операции</param>
    /// <returns>страницу с задачами с примененным фильтром и сортировкой</returns>
    [HttpGet]
    [ProducesResponseType<TasksPageDto>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTasksAsync(
        [FromQuery(Name = "")] PageDto? page,
        [FromQuery] ProjectTaskStatus? status,
        CancellationToken ct = default)
    {
        (int pageNumber, int elementsOnPage) paging = page == null
                ? (0, Constants.DefaultElementsOnPage)
                : (page.Page ?? 0, page.Count ?? Constants.DefaultElementsOnPage);

        logger.LogInformation("Get the page {page} with {items} elements, filtered by status: {status}",
            paging.pageNumber,
            paging.elementsOnPage,
            status);

        return Ok(await taskService.GetTasksAsync(
            paging,
            status,
            ct)
        );
    }

    [HttpPost]
    [ProducesResponseType<ProjectTask>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddTaskAsync([FromBody] CreateTaskDto taskToCreateDto)
    {
        logger.LogInformation("Try to add the task with the name '{name}'",
            taskToCreateDto.Title.Substring(0, Math.Min(taskToCreateDto.Title.Length, 20)));

        var task = mapper.Map<ProjectTask>(taskToCreateDto);

        (ProjectTask? createdTask, IEnumerable<string> errors) = await taskService.AddTaskAsync(task);

        logger.LogInformation("Task created: {IsCreated}, Errors: {Errors}",
            createdTask == null ? "no" : "yes",
            string.Join("\n", errors.Take(5)));

        return errors.Any() ? BadRequest(errors) : Ok(createdTask);
    }

    [HttpPatch]
    [ProducesResponseType<ProjectTask>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateTaskAsync([FromBody] UpdateTaskDto taskToUpdateDto, CancellationToken ct = default)
    {
        logger.LogInformation("Try to update the task with the Id '{Id}'", taskToUpdateDto.Id);

        var task = await taskRepository.GetByIdAsync(taskToUpdateDto.Id, ct);

        if (task == null)
        {
            return NotFound();
        }

        (ProjectTask? updatedTask, IEnumerable<string> errors) = await taskService.UpdateTaskAsync(task, taskToUpdateDto, ct);

        logger.LogInformation("Task updated: {IsUpdated}, Errors: {Errors}",
            updatedTask == null ? "no" : "yes",
            string.Join("\n", errors.Take(5)));

        return errors.Any() ? BadRequest(errors) : Ok(updatedTask);
    }
}
