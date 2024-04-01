using TaskMate.DTOs.Notifaction;

namespace TaskMate.Service.Abstraction;

public interface INotifactionService
{
    Task<List<GetNotifactionDto>> UserGetAllNotifaction(string AppUserId);
    Task SeenNotifaction(string AppUserId);
}
