using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using TaskMate.Context;
using TaskMate.DTOs.Auth;
using TaskMate.DTOs.Boards;
using TaskMate.DTOs.Workspace;
using TaskMate.Entities;
using TaskMate.Exceptions;
using TaskMate.Helper.Enum.Board;
using TaskMate.Helper.Enum.User;
using TaskMate.Service.Abstraction;

namespace TaskMate.Service.Implementations;

public class BoardsService : IBoardsService
{
    private readonly AppDbContext _appDbContext;
    private readonly UserManager<AppUser> _userManager;
    private readonly IWorkspaceService _workspaceService;
    private readonly IAuthService _authService;
    private readonly IMapper _mapper;

    public BoardsService(AppDbContext appDbContext, UserManager<AppUser> userManager, IMapper mapper, IWorkspaceService workspaceService, IAuthService authService)
    {
        _appDbContext = appDbContext;
        _userManager = userManager;
        _mapper = mapper;
        _workspaceService = workspaceService;
        _authService = authService;
    }

    public async Task AddUserBoard(AddUserBoardDto addUserBoard)
    {
        var byGlobalAdmin = await _userManager.FindByIdAsync(addUserBoard.AdminId);

        var adminRol = await _userManager.GetRolesAsync(byGlobalAdmin);

        if (adminRol.FirstOrDefault().ToString() != Role.GlobalAdmin.ToString() &&
            adminRol.FirstOrDefault().ToString() != Role.Admin.ToString())
            throw new PermisionException("No Access");

        var FindUser = await _appDbContext.AppUsers.FirstOrDefaultAsync(x => x.Id == addUserBoard.AppUserId);
        if (FindUser is null)
            throw new NotFoundException("Not Found User");

        var workpsaceUser = await _appDbContext.WorkspaceUsers.Include(x => x.Workspace)
        .FirstOrDefaultAsync(x => x.AppUserId == addUserBoard.AppUserId && x.WorkspaceId == addUserBoard.WorkspaceId);
        if (workpsaceUser is null)
        {
            var addUserWokspace = new AddUserWorkspace()
            {
                AdminId = addUserBoard.AdminId,
                WorkspaceId = addUserBoard.WorkspaceId,
                AppUserId = addUserBoard.AppUserId,
            };
            await _workspaceService.AddUserWorkspace(addUserWokspace);
        }
        var newUserBoard = new UserBoards()
        {
            AppUserId = addUserBoard.AppUserId,
            BoardsId = addUserBoard.BoardId,
        };

        var board = await _appDbContext.Boards.FirstOrDefaultAsync(z => z.Id == addUserBoard.BoardId);

        var newNotification = new Notification()
        {
            WorkspaceId = addUserBoard.WorkspaceId,
            BoardId = addUserBoard.BoardId,
            Text = $"Hello {FindUser.UserName} :) Mr. {byGlobalAdmin.UserName} added you to the {board.Title} board in the new business area called {workpsaceUser.Workspace.Title}.You can go to the workspace by clicking the button.",
            AppUserId = addUserBoard.AppUserId
        };
        var userActivity = new UserActivity()
        {
            AppUserId = addUserBoard.AdminId,
            BoardId = board.Id,
            ActivityText = $"added {FindUser.UserName} to the {board.Title} board"
        };
        await _appDbContext.UserActivityes.AddAsync(userActivity);
        await _appDbContext.Notifications.AddAsync(newNotification);
        await _appDbContext.UserBoards.AddAsync(newUserBoard);
        await _appDbContext.SaveChangesAsync();
    }

    public async Task CreateAsync(CreateBoardsDto createBoardsDto)
    {
        var byAdmin = await _userManager.FindByIdAsync(createBoardsDto.AppUserId);

        var adminRol = await _userManager.GetRolesAsync(byAdmin);

        if (adminRol.FirstOrDefault().ToString() != Role.GlobalAdmin.ToString() &&
            adminRol.FirstOrDefault().ToString() != Role.Admin.ToString())
            throw new PermisionException("No Access");
        var workspace = await _appDbContext.Workspaces.FirstOrDefaultAsync(x => x.Id == createBoardsDto.WorkspaceId);
        if (workspace is null)
            throw new NotFoundException("Not Found Workspace");

        var newBoard = _mapper.Map<Boards>(createBoardsDto);
        await _appDbContext.Boards.AddAsync(newBoard);
        await _appDbContext.SaveChangesAsync();

        var userBoard = new UserBoards()
        {
            AppUserId = createBoardsDto.AppUserId,
            BoardsId = newBoard.Id,
        };

        var userActivity = new UserActivity()
        {
            AppUserId = byAdmin.Id,
            BoardId = newBoard.Id,
            ActivityText = $"{workspace.Title} added a board called {createBoardsDto.Title} to the workspace"
        };
        await _appDbContext.UserActivityes.AddAsync(userActivity);
        await _appDbContext.UserBoards.AddAsync(userBoard);
        await _appDbContext.SaveChangesAsync();

    }

    public async Task<List<GetBoardsDto>> GetAllAsync(string AppUserId, Guid WorkspaceId)
    {
        var user = await _userManager.FindByIdAsync(AppUserId);
        if (user is null) throw new NotFoundException("Not Found User");
        var adminRol = await _userManager.GetRolesAsync(user);

        var workspaces = await _workspaceService.GetAllAsync(AppUserId);
        if (workspaces is null) return null;

        bool isTrue = false;
        foreach (var item in workspaces)
            if (item.Id == WorkspaceId) isTrue = true;



        if (isTrue)
        {
            if (adminRol.FirstOrDefault().ToString() == Role.GlobalAdmin.ToString())
            {
                var GAWokrspaceInBoards = await _appDbContext.Boards.Where(x => x.WorkspaceId == WorkspaceId).ToListAsync();
                return _mapper.Map<List<GetBoardsDto>>(GAWokrspaceInBoards);
            }
            else if (adminRol.FirstOrDefault().ToString() == Role.Admin.ToString())
            {
                var WokrspaceInBoardsAdmin = await _appDbContext.Boards.Where(x => x.WorkspaceId == WorkspaceId).ToListAsync();

                var userAccesBoardAdmin = new List<Boards>();
                foreach (var board in WokrspaceInBoardsAdmin)
                {
                    if (board.BoardAccessibility == BoardAccessibility.Private)
                    {
                        if (await _appDbContext.UserBoards.FirstOrDefaultAsync(x => x.AppUserId == AppUserId && x.BoardsId == board.Id) is not null)
                        {
                            userAccesBoardAdmin.Add(board);
                            continue;
                        }
                    }
                    else
                        userAccesBoardAdmin.Add(board);
                }

                return _mapper.Map<List<GetBoardsDto>>(userAccesBoardAdmin);
            }
            var WokrspaceInBoards = await _appDbContext.Boards.Where(x => x.WorkspaceId == WorkspaceId && x.IsArchive == false).ToListAsync();

            var userAccesBoard = new List<Boards>();
            foreach (var board in WokrspaceInBoards)
            {
                if (board.BoardAccessibility == BoardAccessibility.Private)
                {
                    if (await _appDbContext.UserBoards.FirstOrDefaultAsync(x => x.AppUserId == AppUserId && x.BoardsId == board.Id) is not null)
                    {
                        userAccesBoard.Add(board);
                        continue;
                    }
                }
                else
                    userAccesBoard.Add(board);
            }

            return _mapper.Map<List<GetBoardsDto>>(userAccesBoard);
        }
        else return null;
    }

    public async Task<List<GetBoardInUserDto>> GetBoardAllUser(Guid BoardId)
    {
        var boardInUsers = await _appDbContext.UserBoards
                                .Include(x => x.AppUser)
                                .Where(x => x.BoardsId == BoardId)
                                .Select(x => x.AppUser)
                                .ToListAsync();

        return _mapper.Map<List<GetBoardInUserDto>>(boardInUsers);
    }

    public async Task<List<GetBoardsDto>> GetByIdAsync(Guid BoardId)
    {
        var board = await _appDbContext.Boards.Include(x => x.CardLists)
                               .ThenInclude(x => x.Cards.OrderByDescending(x => x.CreatedDate)).Where(x => x.Id == BoardId).ToListAsync();
        if (board is null)
            throw new NotFoundException("Not Found");

        return _mapper.Map<List<GetBoardsDto>>(board);

    }

    public async Task Remove(string AdminId, Guid BoardId)
    {
        var byAdmin = await _userManager.FindByIdAsync(AdminId);

        var adminRol = await _userManager.GetRolesAsync(byAdmin);

        if (adminRol.FirstOrDefault().ToString() != Role.GlobalAdmin.ToString() &&
            adminRol.FirstOrDefault().ToString() != Role.Admin.ToString())
            throw new PermisionException("No Access");

        var board = await _appDbContext.Boards.Where(x => x.Id == BoardId).FirstOrDefaultAsync();
        if (board is null)
            throw new NotFoundException("Not Found");



        var userActivity = new UserActivity()
        {
            AppUserId = byAdmin.Id,
            BoardId = board.Id,
            ActivityText = $"deleted his {board.Title} board"
        };
        _appDbContext.Boards.Remove(board);
        await _appDbContext.UserActivityes.AddAsync(userActivity);
        await _appDbContext.SaveChangesAsync();
    }

    public async Task ShareLinkBoardToUser(LinkShareToBoardDto linkShareToBoardDto)
    {
        var byAdmin = await _userManager.FindByIdAsync(linkShareToBoardDto.AdminId);

        var adminRol = await _userManager.GetRolesAsync(byAdmin);

        if (adminRol.FirstOrDefault().ToString() != Role.GlobalAdmin.ToString() &&
            adminRol.FirstOrDefault().ToString() != Role.Admin.ToString())
            throw new PermisionException("No Access");

        var tokenResponse = await _authService.ShareLinkToUser(linkShareToBoardDto.UsernameOrEmail, linkShareToBoardDto.Password);
        if (tokenResponse.token is null)
            throw new NotFoundException("Username and password are incorrect");

        var newAddUserBoard = new AddUserBoardDto()
        {
            AdminId = linkShareToBoardDto.AdminId,
            WorkspaceId = linkShareToBoardDto.WorkspaceId,
            BoardId = linkShareToBoardDto.BoardId,
            AppUserId = tokenResponse.appuserid,
        };
        await AddUserBoard(newAddUserBoard);
    }

    public async Task UpdateArchive(string AppUserId, Guid BoardId)
    {
        var byAdmin = await _userManager.FindByIdAsync(AppUserId);

        var adminRol = await _userManager.GetRolesAsync(byAdmin);

        if (adminRol.FirstOrDefault().ToString() != Role.GlobalAdmin.ToString() &&
            adminRol.FirstOrDefault().ToString() != Role.Admin.ToString())
            throw new PermisionException("No Access");

        var board = await _appDbContext.Boards.FirstOrDefaultAsync(x => x.Id == BoardId);
        if (board is null) throw new NotFoundException("Not Found");

        board.IsArchive = !board.IsArchive;

        _appDbContext.Update(board);
        await _appDbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(UpdateBoardsDto updateBoardsDto)
    {
        var byAdmin = await _userManager.FindByIdAsync(updateBoardsDto.AppUserId);

        var adminRol = await _userManager.GetRolesAsync(byAdmin);

        if (adminRol.FirstOrDefault().ToString() != Role.GlobalAdmin.ToString() &&
           adminRol.FirstOrDefault().ToString() != Role.Admin.ToString())
            throw new PermisionException("No Access");

        var board = await _appDbContext.Boards.Where(x => x.Id == updateBoardsDto.BoardId).FirstOrDefaultAsync();
        if (board is null)
            throw new NotFoundException("Not Found");
        string oldBoardTitle = board.Title;

        board.Title = updateBoardsDto.Title;
        var userActivity = new UserActivity()
        {
            AppUserId = byAdmin.Id,
            BoardId = board.Id,
            ActivityText = $"changed the name of the {oldBoardTitle} board and named it {updateBoardsDto.Title}"
        };
        _appDbContext.Boards.Update(board);
        await _appDbContext.UserActivityes.AddAsync(userActivity);
        await _appDbContext.SaveChangesAsync();
    }
}
