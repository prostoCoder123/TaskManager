using System.ComponentModel.DataAnnotations;

namespace TaskManager.Dto;

public class CreateTaskDto
{
    [MinLength(Constants.MinLength3)]
    [MaxLength(Constants.MaxLength100)]
    [RegularExpression(Constants.AllowedCharsRegexTemplate)]
    public string Title { get; init; } = default!;

    [MinLength(Constants.MinLength3)]
    [MaxLength(Constants.MaxLength1000)]
    [RegularExpression(Constants.AllowedCharsRegexTemplate)]
    public string Description { get; init; } = default!;

    [FutureDate]
    public DateTime DueDate { get; init; }
}