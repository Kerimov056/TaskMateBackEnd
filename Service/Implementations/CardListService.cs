using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskMate.Context;
using TaskMate.DTOs.CardList;
using TaskMate.Entities;
using TaskMate.Exceptions;
using TaskMate.Helper.Enum.User;
using TaskMate.Service.Abstraction;

namespace TaskMate.Service.Implementations;

public class CardListService : ICardListService
{
    private readonly AppDbContext _appDbContext;
    private readonly UserManager<AppUser> _userManager;
    private readonly IMapper _mapper;

    public CardListService(AppDbContext appDbContext, UserManager<AppUser> userManager, IMapper mapper)
    {
        _appDbContext = appDbContext;
        _userManager = userManager;
        _mapper = mapper;
    }
    public async Task CreateAsync(CreateCardListDto createCardListDto)
    {
        var user = await _userManager.FindByIdAsync(createCardListDto.AppUserId);
        if (user == null) throw new NotFoundException("User Not Found");

        var isAccess = await UserBoardAccess(createCardListDto.AppUserId, createCardListDto.BoardsId);
        if (isAccess == false) throw new PermisionException("Not Access");

        var board = await _appDbContext.Boards.FirstOrDefaultAsync(x=>x.Id==createCardListDto.BoardsId);
        if (board is null)
            throw new NotFoundException("Not Found Workspace");

        var newcardList = _mapper.Map<CardList>(createCardListDto);
        await _appDbContext.CardLists.AddAsync(newcardList);

        var userActivity = new UserActivity()
        {
            AppUserId = createCardListDto.AppUserId,
            BoardId = createCardListDto.BoardsId,
            ActivityText = $"{board.Title} added a list called {createCardListDto.Title} to the boarding"
        };
        await _appDbContext.UserActivityes.AddAsync(userActivity);
        await _appDbContext.SaveChangesAsync();
    }

    public async Task<List<GetCardListDto>> GetAllCardListAsync(Guid BoardId)
    {
        var board = await _appDbContext.CardLists.Where(x=>x.BoardsId==BoardId).ToListAsync();
        if (board is null) return null;

        return _mapper.Map<List<GetCardListDto>>(board);
    }

    public async Task Remove(string AdminId, Guid CardlistId)
    {
        var byAdmin = await _userManager.FindByIdAsync(AdminId);

        var adminRol = await _userManager.GetRolesAsync(byAdmin);

        if (adminRol.FirstOrDefault().ToString() != Role.GlobalAdmin.ToString() &&
                 adminRol.FirstOrDefault().ToString() != Role.Admin.ToString())
            throw new PermisionException("No Access");

        var cardlist = await _appDbContext.CardLists.Where(x => x.Id == CardlistId).FirstOrDefaultAsync();
        if (cardlist is null)
            throw new NotFoundException("Not Found");

        _appDbContext.CardLists.Remove(cardlist);


        var userActivity = new UserActivity()
        {
            AppUserId = byAdmin.Id,
            BoardId = cardlist.BoardsId,
            ActivityText = $"deleted the list named {cardlist.Title}"
        };
        await _appDbContext.UserActivityes.AddAsync(userActivity);
        await _appDbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(UpdateeCardListDto updateeCardListDto)
    {
        var byAdmin = await _userManager.FindByIdAsync(updateeCardListDto.AppUserId);

        var adminRol = await _userManager.GetRolesAsync(byAdmin);

        if (adminRol.FirstOrDefault().ToString() != Role.GlobalAdmin.ToString() &&
                 adminRol.FirstOrDefault().ToString() != Role.Admin.ToString())
            throw new PermisionException("No Access");

        var cardList = await _appDbContext.CardLists.Where(x => x.Id == updateeCardListDto.CardListId).FirstOrDefaultAsync();
        if (cardList is null)
            throw new NotFoundException("Not Found");
        
        var oldListTitle = cardList.Title;

        cardList.Title = updateeCardListDto.Title;
        _appDbContext.CardLists.Update(cardList);

        var userActivity = new UserActivity()
        {
            AppUserId = byAdmin.Id,
            BoardId = cardList.BoardsId,
            ActivityText = $"changed the name of the list named {oldListTitle} to {cardList.Title}"
        };
        await _appDbContext.UserActivityes.AddAsync(userActivity);
        await _appDbContext.SaveChangesAsync();
    }

    public async Task<bool> UserBoardAccess(string AppUserId, Guid BoardId)
    {
        var userAccess = await _appDbContext.UserBoards.FirstOrDefaultAsync(x=>x.AppUserId==AppUserId
                                                        && x.BoardsId == BoardId);

        if (userAccess is null) return false;
        return true;
    }
}
