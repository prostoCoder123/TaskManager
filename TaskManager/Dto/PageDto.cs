using System.ComponentModel.DataAnnotations;

namespace TaskManager.Dto;

public class PageDto
{
    [Range(0, int.MaxValue)]
    public int? Page { get; init; }

    [Range(1, Constants.MaxElementsOnPage)]
    public int? Count { get; init; }
}
