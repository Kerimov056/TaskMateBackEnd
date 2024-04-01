using AutoMapper;
using TaskMate.DTOs.Auth;
using TaskMate.Entities;

namespace TaskMate.MapperProfile.Auth;

public class AppUserProfile : Profile
{
    public AppUserProfile()
    {
        CreateMap<AppUser, GetBoardInUserDto>().ReverseMap();
    }
}
