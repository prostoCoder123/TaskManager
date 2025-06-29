using Entities;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Abstractions;
using TaskManager.Dto;

namespace TaskManager.Controllers;

[ApiController]
[Route("[controller]")]
public class TasksController(ITaskService taskService, ILogger<TasksController> logger) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<TasksPageDto>(StatusCodes.Status200OK)]
    public IActionResult Get(
        [FromQuery] PageDto page,
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

        return Ok(
            taskService.GetTasksAsync(
                paging,
                status,
                ct)
        );
    }

    [HttpPost]
    public IActionResult AddTask([FromBody] CreateTaskDto taskToCreate)
    {
        return Ok(taskToCreate);
    }
}
