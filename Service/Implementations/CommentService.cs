using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.Design;
using TaskMate.Context;
using TaskMate.DTOs.Checkitem;
using TaskMate.DTOs.Comment;
using TaskMate.Entities;
using TaskMate.Exceptions;
using TaskMate.Helper.Enum.User;
using TaskMate.Service.Abstraction;

namespace TaskMate.Service.Implementations;

public class CommentService : ICommentService
{
    private readonly AppDbContext _appDbContext;
    private readonly UserManager<AppUser> _userManager;
    private readonly IWorkspaceService _workspaceService;
    private readonly IAuthService _authService;
    private readonly IMapper _mapper;

    public CommentService(AppDbContext appDbContext, UserManager<AppUser> userManager, IMapper mapper, IWorkspaceService workspaceService, IAuthService authService)
    {
        _appDbContext = appDbContext;
        _userManager = userManager;
        _mapper = mapper;
        _workspaceService = workspaceService;
        _authService = authService;
    }

    public async Task CreateAsync(CreateCommentDto createCommentDto)
    {
        var newComment = _mapper.Map<Comment>(createCommentDto);

        var card = await _appDbContext.Cards.FirstOrDefaultAsync(x => x.Id == createCommentDto.CardId);
        var board = await _appDbContext.Boards.Include(x => x.CardLists).ThenInclude(x => x.Cards.Where(x => x.Id == createCommentDto.CardId)).FirstOrDefaultAsync();

        await _appDbContext.Comments.AddAsync(newComment);
        var userActivity = new UserActivity()
        {
            AppUserId = createCommentDto.AppUserId,
            BoardId = board.Id,
            CardId = createCommentDto.CardId,
            ActivityText = $"Here: {card.Title}: {createCommentDto.Message}"
        };
        await _appDbContext.UserActivityes.AddAsync(userActivity);
        await _appDbContext.SaveChangesAsync();
    }

    public async Task<List<GetCommentDto>> GetByCardComments(Guid CardId)
    {
        var card = await _appDbContext.Cards.FirstOrDefaultAsync(x => x.Id == CardId);
        if (card is null)
            throw new NotFoundException("Not Found Card");

        var Comments = await _appDbContext.Comments.Where(x => x.CardId == CardId).OrderByDescending(x=>x.CreatedDate).ToListAsync();
        return _mapper.Map<List<GetCommentDto>>(Comments);
    }

    public async Task RemoveAsync(string AppUserId, Guid CommentId)
    {
        var comment = await _appDbContext.Comments.FirstOrDefaultAsync(x => x.AppUserId == AppUserId && x.Id == CommentId);
        var baord = await _appDbContext.Boards.Include(x => x.CardLists)
                    .ThenInclude(x => x.Cards).ThenInclude(x => x.Comments.Where(x => x.Id == CommentId))
                    .FirstOrDefaultAsync();
        bool isRemove = false;
        if (comment is not null)
        {
            var userActivity = new UserActivity()
            {
                AppUserId = AppUserId,
                BoardId = baord.Id,
                CardId = comment.Id,
                ActivityText = $"Comment Remove"
            };
            await _appDbContext.UserActivityes.AddAsync(userActivity);
            _appDbContext.Comments.Remove(comment);
            await _appDbContext.SaveChangesAsync();
            isRemove = true;
        }
        if (isRemove is false)
        {
            var adminAccesComment = await _appDbContext.Comments.FirstOrDefaultAsync(x => x.Id == CommentId);

            var byAdmin = await _userManager.FindByIdAsync(AppUserId);

            var adminRol = await _userManager.GetRolesAsync(byAdmin);

            if (adminRol.FirstOrDefault().ToString() == Role.GlobalAdmin.ToString() ||
               adminRol.FirstOrDefault().ToString() == Role.Admin.ToString())
            {
                var userActivity = new UserActivity()
                {
                    AppUserId = byAdmin.Id,
                    BoardId = baord.Id,
                    CardId = adminAccesComment.CardId,
                    ActivityText = $"Comment Remove"
                };
                await _appDbContext.UserActivityes.AddAsync(userActivity);
                _appDbContext.Comments.Remove(adminAccesComment);
                await _appDbContext.SaveChangesAsync();
            }
            else
            {
                throw new PermisionException("No Access");
            }
        }
    }

    public async Task UpdateAsync(UpdateCommentDto updateCommentDto)
    {
        var comment = await _appDbContext.Comments.FirstOrDefaultAsync(x => x.Id == updateCommentDto.Id && x.AppUserId == updateCommentDto.AppUserId);
        if (comment is null)
            throw new NotFoundException("Not Found Comment");

        var baord = await _appDbContext.Boards.Include(x => x.CardLists)
                    .ThenInclude(x => x.Cards).ThenInclude(x => x.Comments.Where(x => x.Id == updateCommentDto.Id))
                    .FirstOrDefaultAsync();

        _mapper.Map(updateCommentDto, comment);

        var userActivity = new UserActivity()
        {
            AppUserId = updateCommentDto.AppUserId,
            BoardId = baord.Id,
            CardId = comment.CardId,
            ActivityText = $"Comment Update: {updateCommentDto.Message}"
        };
        await _appDbContext.UserActivityes.AddAsync(userActivity);
        _appDbContext.Comments.Update(comment);
        await _appDbContext.SaveChangesAsync();
    }
}
