using AutoMapper;
using TaskMate.DTOs.Notifaction;
using TaskMate.Entities;

namespace TaskMate.MapperProfile.NotifactionP;

public class NotifactionProfile:Profile
{
    public NotifactionProfile()
    {
        CreateMap<Notification, GetNotifactionDto>().ReverseMap();
    }
}
