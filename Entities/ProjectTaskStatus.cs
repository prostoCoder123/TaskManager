using System.Text.Json.Serialization;

namespace Entities;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ProjectTaskStatus
{
    [JsonStringEnumMemberName("Новая")]
    New,

    [JsonStringEnumMemberName("В работе")]
    InProgress,

    [JsonStringEnumMemberName("Завершена")]
    Completed,

    [JsonStringEnumMemberName("Просрочена")]
    OverDue
}
