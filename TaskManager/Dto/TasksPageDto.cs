using Entities;

namespace TaskManager.Dto
{
    public class TasksPageDto
    {
        public IAsyncEnumerable<ProjectTask> Tasks { get; init; } = default!;
        public int Total { get; init; } = default!;
        public int PageNumber { get; init; } = default!;
    }
}