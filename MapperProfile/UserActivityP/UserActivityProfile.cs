using AutoMapper;
using TaskMate.DTOs.UserActivityD;
using TaskMate.Entities;

namespace TaskMate.MapperProfile.UserActivityP;

public class UserActivityProfile:Profile
{
    public UserActivityProfile()
    {
        CreateMap<UserActivity, GetUserActivityDto>().ReverseMap();
    }
}
