using AutoMapper;
using TaskPlanner.Application.DTOs;
using TaskPlanner.Application.Requests.Users;
using TaskPlanner.Domain.Entities;

namespace TaskPlanner.Application.Mappings;

public sealed class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDto>();
        CreateMap<CreateUserRequest, User>();
    }
}
