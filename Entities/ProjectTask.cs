using System.ComponentModel.DataAnnotations;

namespace Entities;

/// <summary>
/// Сущность задачи
/// </summary>
public class ProjectTask
{
    /// <summary>
    /// Уникальный идентификатор задачи
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Название задачи
    /// </summary>
    [Required]
    public string Title { get; set; } = default!;

    /// <summary>
    /// Описание задачи
    /// </summary>
    [Required]
    public string Description { get; set; } = default!;

    /// <summary>
    /// Статус выполнения задачи
    /// </summary>
    public ProjectTaskStatus Status { get; set; }

    /// <summary>
    /// Дата и время срока, к которому требуется завершить задачу
    /// </summary>
    public DateTime DueDate { get; set; }

    /// <summary>
    /// Дата и время создания задачи
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Дата и время последнего обновления задачи
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Дата и время завершения задачи
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    // TODO: who created, updated, etc.
}