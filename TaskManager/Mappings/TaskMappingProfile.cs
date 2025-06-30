using AutoMapper;
using Entities;
using TaskManager.Dto;

namespace TaskManager.Mappings;

public class TaskMappingProfile : Profile
{
    public TaskMappingProfile()
    {
        CreateMap<CreateTaskDto, ProjectTask>()
            .ForMember(m => m.Id, e => e.Ignore())
            .ForMember(m => m.DueDate, e => e.MapFrom(v => v.DueDate.ToUniversalTime()))
            .ForMember(m => m.Title, e => e.MapFrom(v => v.Title.Trim()))
            .ForMember(m => m.Description, e => e.MapFrom(v => v.Description.Trim()))
            .ForMember(m => m.CreatedAt, e => e.MapFrom(v => DateTime.UtcNow))
            .ForMember(m => m.UpdatedAt, e => e.MapFrom(v => DateTime.UtcNow))
            .ForMember(m => m.Status, e => e.MapFrom(v => ProjectTaskStatus.New));
    }
}
