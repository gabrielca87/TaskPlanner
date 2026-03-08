using AutoMapper;
using TaskPlanner.Application.DTOs;
using TaskPlanner.Application.Requests.Tasks;
using TaskPlanner.Domain.Entities;

namespace TaskPlanner.Application.Mappings;

public class TaskItemProfile : Profile
{
    public TaskItemProfile()
    {
        CreateMap<TaskItem, TaskItemDto>();
        CreateMap<CreateTaskItemRequest, TaskItem>();
    }
}
