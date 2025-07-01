using AutoMapper;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.ComponentModel.DataAnnotations;
using TaskManager.Abstractions;
using TaskManager.Controllers;
using TaskManager.Dto;

namespace TaskManager.Tests
{
    public class TaskControllerTests
    {
        [Fact]
        public void TryValidateObject_ReturnsError_WhenTitleIsInvalid()
        {
            // Arrange
            var taskDto = new CreateTaskDto()
            {
                Title = "New task %8 = /console", // illegal chars
                Description = "Description",
                DueDate = DateTime.Now.AddDays(10),
            };

            // Act
            var lstErrors = ValidateModel(taskDto);

            // Assert
            Assert.Single(lstErrors);
        }

        [Fact]
        public async Task GetById_ReturnsTask_WhenRequestedTaskExists()
        {
            // Arrange
            var task = new ProjectTask() { Id = 1, Title = "Update dependencies", Description = "Reinstall and update NuGet packages", CreatedAt = new(2025, 3, 13, 10, 0, 0, DateTimeKind.Utc), Status = ProjectTaskStatus.Completed, CompletedAt = new(2025, 3, 15, 12, 8, 3, DateTimeKind.Utc), DueDate = new(2025, 3, 15, 15, 0, 0, DateTimeKind.Utc), UpdatedAt = new(2025, 3, 15, 12, 8, 3, DateTimeKind.Utc) };

            var mockRepo = new Mock<ITaskRepository>();
            mockRepo.Setup(r => r.GetByIdAsync(It.IsIn(task.Id), It.IsAny<CancellationToken>())).ReturnsAsync(task);

            // Act
            var result = await mockRepo.Object.GetByIdAsync(task.Id, default);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(task.Id, result.Id);
            Assert.Equal(task.Title, result.Title);
            mockRepo.Verify(m => m.GetByIdAsync(task.Id, default), Times.Once());
        }

        [Fact]
        public async Task GetById_ReturnsNull_WhenRequestedTaskDoesNotExist()
        {
            // Arrange
            var task = new ProjectTask() { Id = 1, Title = "Update dependencies", Description = "Reinstall and update NuGet packages", CreatedAt = new(2025, 3, 13, 10, 0, 0, DateTimeKind.Utc), Status = ProjectTaskStatus.Completed, CompletedAt = new(2025, 3, 15, 12, 8, 3, DateTimeKind.Utc), DueDate = new(2025, 3, 15, 15, 0, 0, DateTimeKind.Utc), UpdatedAt = new(2025, 3, 15, 12, 8, 3, DateTimeKind.Utc) };

            var mockRepo = new Mock<ITaskRepository>();
            mockRepo.Setup(r => r.GetByIdAsync(It.IsIn(task.Id), It.IsAny<CancellationToken>())).ReturnsAsync(task);

            int missingId = 5;

            // Act
            var result = await mockRepo.Object.GetByIdAsync(missingId, default);

            // Assert
            Assert.Null(result);
            mockRepo.Verify(m => m.GetByIdAsync(missingId, default), Times.Once());
            mockRepo.Verify(m => m.GetByIdAsync(task.Id, default), Times.Never());
        }

        [Fact]
        public async Task GetTaskById_ReturnsOkObjectResult_WhenTaskExists()
        {
            // Arrange
            var task = new ProjectTask() { Id = 1, Title = "Update dependencies", Description = "Reinstall and update NuGet packages", CreatedAt = new(2025, 3, 13, 10, 0, 0, DateTimeKind.Utc), Status = ProjectTaskStatus.Completed, CompletedAt = new(2025, 3, 15, 12, 8, 3, DateTimeKind.Utc), DueDate = new(2025, 3, 15, 15, 0, 0, DateTimeKind.Utc), UpdatedAt = new(2025, 3, 15, 12, 8, 3, DateTimeKind.Utc) };

            var mockService = new Mock<ITaskService>();
            mockService.Setup(r => r.GetTaskByIdAsync(It.IsIn(task.Id), It.IsAny<CancellationToken>())).ReturnsAsync(task);

            var mockRepo = new Mock<ITaskRepository>();
            var mockMapper = new Mock<IMapper>();
            var mockLogger = new Mock<ILogger<TasksController>>();

            var controller = new TasksController(mockService.Object, mockMapper.Object, mockRepo.Object, mockLogger.Object);

            // Act
            var result = await controller.GetTaskByIdAsync(task.Id, default);

            // Assert
            var OkResult = Assert.IsType<OkObjectResult>(result);
            mockService.Verify(m => m.GetTaskByIdAsync(task.Id, default), Times.Once());
            mockRepo.Verify(m => m.GetByIdAsync(task.Id, default), Times.Never());
        }

        [Fact]
        public async Task GetTaskById_ReturnsNotFoundResult_WhenTaskDoesNotExist()
        {
            // Arrange
            var task = new ProjectTask() { Id = 1, Title = "Update dependencies", Description = "Reinstall and update NuGet packages", CreatedAt = new(2025, 3, 13, 10, 0, 0, DateTimeKind.Utc), Status = ProjectTaskStatus.Completed, CompletedAt = new(2025, 3, 15, 12, 8, 3, DateTimeKind.Utc), DueDate = new(2025, 3, 15, 15, 0, 0, DateTimeKind.Utc), UpdatedAt = new(2025, 3, 15, 12, 8, 3, DateTimeKind.Utc) };

            var mockService = new Mock<ITaskService>();
            mockService.Setup(r => r.GetTaskByIdAsync(It.IsIn(task.Id), It.IsAny<CancellationToken>())).ReturnsAsync(task);

            var mockRepo = new Mock<ITaskRepository>();
            var mockMapper = new Mock<IMapper>();
            var mockLogger = new Mock<ILogger<TasksController>>();

            var controller = new TasksController(mockService.Object, mockMapper.Object, mockRepo.Object, mockLogger.Object);
            var missingId = 5;

            // Act
            var result = await controller.GetTaskByIdAsync(missingId, default);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
            mockService.Verify(m => m.GetTaskByIdAsync(missingId, default), Times.Once());
            mockRepo.Verify(m => m.GetByIdAsync(missingId, default), Times.Never());
        }

        private IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var ctx = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, ctx, validationResults, true);
            return validationResults;
        }
    }
}