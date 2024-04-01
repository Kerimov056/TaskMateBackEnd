using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TaskMate.Context;
using TaskMate.DTOs;
using TaskMate.DTOs.Boards;
using TaskMate.DTOs.Card;
using TaskMate.DTOs.CardList;
using TaskMate.DTOs.Users;
using TaskMate.DTOs.Workspace;
using TaskMate.Entities;
using TaskMate.Exceptions;
using TaskMate.Helper.Enum.User;
using TaskMate.Service.Abstraction;

namespace TaskMate.Service.Implementations;

public class WorkspaceService : IWorkspaceService
{
    private readonly AppDbContext _appDbContext;
    private readonly UserManager<AppUser> _userManager;
    private readonly IAuthService _authService;
    private readonly IMapper _mapper;

    public WorkspaceService(AppDbContext appDbContext, UserManager<AppUser> userManager, IMapper mapper, IAuthService authService)
    {
        _appDbContext = appDbContext;
        _userManager = userManager;
        _mapper = mapper;
        _authService = authService;
    }

    public async Task AddUserWorkspace(AddUserWorkspace addUserWorkspace)
    {
        var byAdmin = await _userManager.FindByIdAsync(addUserWorkspace.AdminId);

        var adminRol = await _userManager.GetRolesAsync(byAdmin);

        if (adminRol.FirstOrDefault().ToString() != Role.GlobalAdmin.ToString() &&
            adminRol.FirstOrDefault().ToString() != Role.Admin.ToString())
            throw new PermisionException("No Access");

        var user = await _appDbContext.AppUsers.FirstOrDefaultAsync(x => x.Id == addUserWorkspace.AppUserId);
        if (user is null)
            throw new NotFoundException("Not Found User");

        var newWorkspaceUser = new WorkspaceUser()
        {
            AppUserId = addUserWorkspace.AppUserId,
            WorkspaceId = addUserWorkspace.WorkspaceId,
        };

        await _appDbContext.WorkspaceUsers.AddAsync(newWorkspaceUser);

        var workspace = await _appDbContext.Workspaces.FirstOrDefaultAsync(x => x.Id == addUserWorkspace.WorkspaceId);

        var newNotification = new Notification()
        {
            WorkspaceId = addUserWorkspace.WorkspaceId,
            Text = $"Hello {user.UserName} :) Mr. {byAdmin.UserName} added a new business area called {workspace.Title}.You can go to the workspace by clicking the button.",
            AppUserId = addUserWorkspace.AppUserId
        };

        var userActivity = new UserActivity()
        {
            AppUserId = byAdmin.Id,
            ActivityText = $"added {user.UserName} to the {workspace.Title} workspace"
        };
        await _appDbContext.UserActivityes.AddAsync(userActivity);
        await _appDbContext.Notifications.AddAsync(newNotification);
        await _appDbContext.SaveChangesAsync();
    }

    public async Task CreateAsync(CreateWorkspaceDto createWorkspaceDto)
    {
        var byGlobalAdmin = await _userManager.FindByIdAsync(createWorkspaceDto.AppUserId);
        var adminRol = await _userManager.GetRolesAsync(byGlobalAdmin);

        if (adminRol.FirstOrDefault().ToString() != Role.GlobalAdmin.ToString())
            throw new PermisionException("No Access");

        var newWokspace = _mapper.Map<Workspace>(createWorkspaceDto);
        await _appDbContext.Workspaces.AddAsync(newWokspace);

        var userActivity = new UserActivity()
        {
            AppUserId = byGlobalAdmin.Id,
            ActivityText = $"created a workspace called a new {newWokspace.Title}"
        };
        await _appDbContext.UserActivityes.AddAsync(userActivity);
        await _appDbContext.SaveChangesAsync();
    }

    public async Task<List<GetWorkspaceDto>> GetAllAsync(string AppUserId)
    {
        var byAdmin = await _userManager.FindByIdAsync(AppUserId);
        if (byAdmin is null) throw new NotFoundException("Not Found User");

        var adminRol = await _userManager.GetRolesAsync(byAdmin);

        if (adminRol.FirstOrDefault().ToString() == Role.GlobalAdmin.ToString())
        {
            var AllWokrspace = await _appDbContext.Workspaces.ToListAsync();
            var AllWokrspaceToMapper = _mapper.Map<List<GetWorkspaceDto>>(AllWokrspace);

            return AllWokrspaceToMapper;
        }
        else if (adminRol.FirstOrDefault().ToString() == Role.Admin.ToString())
        {
            var UserConnectWorkspaceAdmin = await _appDbContext.Workspaces
                                             .Include(w => w.WorkspaceUsers)
                                             .ThenInclude(wu => wu.AppUser)
                                             .Where(w => w.WorkspaceUsers.Any(wu => wu.AppUserId == AppUserId))
                                             .ToListAsync();

            var UserConnrctWorkspaceToMapperAdmin = _mapper.Map<List<GetWorkspaceDto>>(UserConnectWorkspaceAdmin);

            return UserConnrctWorkspaceToMapperAdmin;
        }
        var UserConnectWorkspace = await _appDbContext.Workspaces
                                            .Include(w => w.WorkspaceUsers)
                                            .ThenInclude(wu => wu.AppUser)
                                            .Where(w => w.WorkspaceUsers.Any(wu => wu.AppUserId == AppUserId))
                                            .Where(x => x.IsArchive == false)
                                            .ToListAsync();

        var UserConnrctWorkspaceToMapper = _mapper.Map<List<GetWorkspaceDto>>(UserConnectWorkspace);

        return UserConnrctWorkspaceToMapper;
    }

    public async Task<List<GetWorkspaceDto>> GetArchiveAsync(string AppUserId)
    {
        var byAdmin = await _userManager.FindByIdAsync(AppUserId);
        if (byAdmin is null) throw new NotFoundException("Not Found User");

        var adminRol = await _userManager.GetRolesAsync(byAdmin);

        if (adminRol.FirstOrDefault().ToString() == Role.GlobalAdmin.ToString())
        {
            var AllWokrspace = await _appDbContext.Workspaces.Where(x => x.IsArchive == true).ToListAsync();
            var AllWokrspaceToMapper = _mapper.Map<List<GetWorkspaceDto>>(AllWokrspace);

            return AllWokrspaceToMapper;
        }
        else if (adminRol.FirstOrDefault().ToString() == Role.Admin.ToString())
        {
            var UserConnectWorkspace = await _appDbContext.Workspaces
                                            .Include(w => w.WorkspaceUsers)
                                            .ThenInclude(wu => wu.AppUser)
                                            .Where(w => w.WorkspaceUsers.Any(wu => wu.AppUserId == AppUserId))
                                            .Where(x => x.IsArchive == true)
                                            .ToListAsync();

            var UserConnrctWorkspaceToMapper = _mapper.Map<List<GetWorkspaceDto>>(UserConnectWorkspace);

            return UserConnrctWorkspaceToMapper;
        }
        else return null;
    }

    public async Task<GetWorkspaceDto> GetByIdAsync(Guid WorspaceId)
    {
        var worksPace = await _appDbContext.Workspaces.Where(x => x.Id == WorspaceId).FirstOrDefaultAsync();
        if (worksPace is null)
            throw new NotFoundException("Not Found");

        return _mapper.Map<GetWorkspaceDto>(worksPace);
    }

    public async Task<GetGlobalSearch> GetGlobalSearch(string searchTerm)
    {
        var getAllWorkpsaces = await _appDbContext.Workspaces.Where(x => x.Title.Contains(searchTerm)).Take(12).ToListAsync();
        var getAllBoards = await _appDbContext.Boards.Where(x => x.Title.Contains(searchTerm)).Take(12).ToListAsync();
        var getAllCardLists = await _appDbContext.CardLists.Where(x => x.Title.Contains(searchTerm)).Take(12).ToListAsync();
        var getAllCards = await _appDbContext.Cards.Where(x => x.Title.Contains(searchTerm)).Take(12).ToListAsync();
        var getAllUsers = await _appDbContext.Users.Where(x => x.UserName.Contains(searchTerm) || x.Fullname.Contains(searchTerm)).Take(12).ToListAsync();

        var workspaceDto = _mapper.Map<List<GetWorkspaceDto>>(getAllWorkpsaces);
        var boardDto = _mapper.Map<List<GetBoardsDto>>(getAllBoards);
        var cardlistDto = _mapper.Map<List<GetThisCardListDto>>(getAllCardLists);
        var cardDto = _mapper.Map<List<GetCardDto>>(getAllCards);
        var userDto = _mapper.Map<List<GetUserDto>>(getAllUsers);

        var globalsearch = new GetGlobalSearch()
        {
            GetWorkspaceDtos = workspaceDto,
            GetBoardsDtos = boardDto,
            GetCardListDtos = cardlistDto,
            GetCardDtos = cardDto,
            GetUserDtos = userDto,
        };

        return globalsearch;
    }

    public async Task<List<GetWorkspaceInBoardDto>> GetWorkspaceInBoards(string AppUserId)
    {
        var byAdmin = await _userManager.FindByIdAsync(AppUserId);

        var adminRol = await _userManager.GetRolesAsync(byAdmin);

        if (adminRol.FirstOrDefault().ToString() != Role.GlobalAdmin.ToString() &&
            adminRol.FirstOrDefault().ToString() != Role.Admin.ToString())
            throw new PermisionException("No Access");

        var workspacInboards = await _appDbContext.Workspaces.Include(x => x.Boards).ToListAsync();

        return _mapper.Map<List<GetWorkspaceInBoardDto>>(workspacInboards);
    }

    public async Task Remove(string AppUserId, Guid WokspaceId)
    {
        var byGlobalAdmin = await _userManager.FindByIdAsync(AppUserId);

        var adminRol = await _userManager.GetRolesAsync(byGlobalAdmin);

        if (adminRol.FirstOrDefault().ToString() != Role.GlobalAdmin.ToString())
            throw new PermisionException("No Access");

        var worksPace = await _appDbContext.Workspaces.Where(x => x.Id == WokspaceId).FirstOrDefaultAsync();
        if (worksPace is null)
            throw new NotFoundException("Not Found");



        var userActivity = new UserActivity()
        {
            AppUserId = byGlobalAdmin.Id,
            ActivityText = $"deleted his {worksPace.Title} workspace"
        };
        _appDbContext.Workspaces.Remove(worksPace);
        await _appDbContext.UserActivityes.AddAsync(userActivity);
        await _appDbContext.SaveChangesAsync();
    }


    public async Task ShareLinkBoardToUser(LinkShareToWorkspaceDto linkShareToWorkspaceDto)
    {
        var byAdmin = await _userManager.FindByIdAsync(linkShareToWorkspaceDto.AdminId);

        var adminRol = await _userManager.GetRolesAsync(byAdmin);

        if (adminRol.FirstOrDefault().ToString() != Role.GlobalAdmin.ToString() &&
            adminRol.FirstOrDefault().ToString() != Role.Admin.ToString())
            throw new PermisionException("No Access");

        var tokenResponse = await _authService.ShareLinkToUser(linkShareToWorkspaceDto.UsernameOrEmail, linkShareToWorkspaceDto.Password);
        if (tokenResponse.token is null)
            throw new NotFoundException("Username and password are incorrect");

        var newAddUserWorkspace = new AddUserWorkspace()
        {
            AdminId = linkShareToWorkspaceDto.AdminId,
            WorkspaceId = linkShareToWorkspaceDto.WorkspaceId,
            AppUserId = tokenResponse.appuserid
        };

        await AddUserWorkspace(newAddUserWorkspace);
    }

    public async Task UpdateArchive(string AppUserId, Guid WorkspaceId)
    {
        var byAdmin = await _userManager.FindByIdAsync(AppUserId);

        var adminRol = await _userManager.GetRolesAsync(byAdmin);

        if (adminRol.FirstOrDefault().ToString() != Role.GlobalAdmin.ToString() &&
            adminRol.FirstOrDefault().ToString() != Role.Admin.ToString())
            throw new PermisionException("No Access"); 

        var workspace = await _appDbContext.Workspaces.FirstOrDefaultAsync(x => x.Id == WorkspaceId);
        if (workspace is null) throw new NotFoundException("Not Found");

        workspace.IsArchive = !workspace.IsArchive;

        _appDbContext.Update(workspace);
        await _appDbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(UpdateWorkspaceDto updateWorkspaceDto)
    {
        var byGlobalAdmin = await _userManager.FindByIdAsync(updateWorkspaceDto.AppUserId);

        var adminRol = await _userManager.GetRolesAsync(byGlobalAdmin);

        if (adminRol.FirstOrDefault().ToString() != Role.GlobalAdmin.ToString())
            throw new PermisionException("No Access");

        var worksPace = await _appDbContext.Workspaces.Where(x => x.Id == updateWorkspaceDto.WorkspaceId).FirstOrDefaultAsync();
        if (worksPace is null)
            throw new NotFoundException("Workspace not found");

        _mapper.Map(updateWorkspaceDto, worksPace);
        string oldWorkspaceTitle = worksPace.Title;
        if (worksPace.Title != "" || worksPace.Title is not null)
        {
            worksPace.Title = updateWorkspaceDto.Title;
        }
        if (worksPace.Description is not null || worksPace.Title != "")
        {
            worksPace.Description = updateWorkspaceDto.Description;
        }

        var userActivity = new UserActivity()
        {
            AppUserId = byGlobalAdmin.Id,
            ActivityText = $"changed the name of the {oldWorkspaceTitle} workspace and named it {worksPace.Title}"
        };
        _appDbContext.Workspaces.Update(worksPace);
        await _appDbContext.UserActivityes.AddAsync(userActivity);
        await _appDbContext.SaveChangesAsync();
    }
}

