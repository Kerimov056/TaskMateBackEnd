using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TaskMate.Context;
using TaskMate.DTOs.Notifaction;
using TaskMate.Service.Abstraction;

namespace TaskMate.Service.Implementations;

public class NotifactionService : INotifactionService
{
    private readonly AppDbContext _appDbContext;
    private readonly IMapper _mapper;

    public NotifactionService(AppDbContext appDbContext, IMapper mapper)
    {
        _appDbContext = appDbContext;
        _mapper = mapper;
    }
    public async Task SeenNotifaction(string AppUserId)
    {
        var allNotifaction = await _appDbContext.Notifications.Where(x => x.IsSeen == false).ToListAsync();
        allNotifaction.ForEach(x => x.IsSeen = true);

        _appDbContext.Notifications.UpdateRange(allNotifaction);
        await _appDbContext.SaveChangesAsync();
    }

    public async Task<List<GetNotifactionDto>> UserGetAllNotifaction(string AppUserId)
    {
        var allNotifaction = await _appDbContext.Notifications.Where(x => x.IsSeen == false && x.AppUserId==AppUserId).ToListAsync();

        return _mapper.Map<List<GetNotifactionDto>>(allNotifaction);
    }
}
