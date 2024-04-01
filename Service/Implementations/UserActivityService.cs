using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TaskMate.Context;
using TaskMate.DTOs.UserActivityD;
using TaskMate.Entities;
using TaskMate.Service.Abstraction;

namespace TaskMate.Service.Implementations;

public class UserActivityService : IUserActivityService
{
    private readonly AppDbContext _appDbContext;
    private readonly IMapper _mapper;
    public UserActivityService(AppDbContext appDbContext, IMapper mapper)
    {
        _appDbContext = appDbContext;
        _mapper = mapper;
    }

    public async Task<List<GetUserActivityDto>> GetCardActivity(Guid CardId)
    {
        var userBoardInActivity = await _appDbContext.UserActivityes.Include(x=>x.AppUser)
                               .Where(x => x.CardId == CardId).ToListAsync();

        var toMapper = _mapper.Map<List<GetUserActivityDto>>(userBoardInActivity);
        foreach (var item in userBoardInActivity)
        {
            foreach (var mapper in toMapper)
            {
                mapper.UserName = item.AppUser.UserName;
            }
        }
        return toMapper;
    }

    public async Task<List<UserActivity>> GetUserBoardActivity(string AppUserId, Guid BoardId)
    {
        var userBoardInActivity = await _appDbContext.UserActivityes
                                .Where(x => x.BoardId == BoardId && x.AppUserId == AppUserId).ToListAsync();

        return userBoardInActivity;
    }
}
