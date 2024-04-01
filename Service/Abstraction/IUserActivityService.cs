using TaskMate.DTOs.UserActivityD;
using TaskMate.Entities;

namespace TaskMate.Service.Abstraction;

public interface IUserActivityService
{
    Task<List<UserActivity>> GetUserBoardActivity(string AppUserId, Guid BoardId);
    Task<List<GetUserActivityDto>> GetCardActivity(Guid CardId);
}
