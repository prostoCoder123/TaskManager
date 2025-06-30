using Entities;
using System.ComponentModel.DataAnnotations;

namespace TaskManager.Dto;

public class UpdateTaskDto
{
    [Range(0, int.MaxValue)]
    [Required]
    public int? Id { get; init; }

    [MinLength(Constants.MinLength3)]
    [MaxLength(Constants.MaxLength100)]
    //TODO: regular expr
    public string? Title { get; init; } = default!;

    [MinLength(Constants.MinLength3)]
    [MaxLength(Constants.MaxLength1000)]
    //TODO: regular expr
    public string? Description { get; init; } = default!;

    [FutureDate]
    public DateTime? DueDate { get; init; } = null;

    [AllowedValues(null, ProjectTaskStatus.InProgress, ProjectTaskStatus.Completed)]
    public ProjectTaskStatus? Status { get; init; } = null;

    public bool HasChanges() => Title != null && Description != null && Status != null && DueDate != null;
}
