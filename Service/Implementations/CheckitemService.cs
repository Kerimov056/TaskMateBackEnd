using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TaskMate.Context;
using TaskMate.DTOs.Checkitem;
using TaskMate.Entities;
using TaskMate.Exceptions;
using TaskMate.Service.Abstraction;

namespace TaskMate.Service.Implementations;

public class CheckitemService : ICheckitemService
{
    private readonly AppDbContext _appDbContext;
    private readonly IMapper _mapper;
    public CheckitemService(AppDbContext appDbContext, IMapper mapper)
    {
        _appDbContext = appDbContext;
        _mapper = mapper;
    }
    public async Task CreateAsync(CreateCheckitemDto createCheckitemDto)
    {
        var checklist = await _appDbContext.Checklists.FirstOrDefaultAsync(x => x.Id == createCheckitemDto.ChecklistId);
        if (checklist is null)
            throw new NotFoundException("Not Found");

        var newCheckItem = _mapper.Map<Checkitem>(createCheckitemDto);

        await _appDbContext.Checkitems.AddAsync(newCheckItem);

        var board = await _appDbContext.Boards.Include(x => x.CardLists)
                   .ThenInclude(x => x.Cards).ThenInclude(x => x.Checklists.Where(x => x.Id == checklist.Id))
                   .FirstOrDefaultAsync();
        var userActivity = new UserActivity()
        {
            AppUserId = createCheckitemDto.AppUserId,
            BoardId = board.Id,
            CardId = checklist.CardId,
            ActivityText = $"added {createCheckitemDto.Text} check item to {checklist.Name} checklist"
        };
        await _appDbContext.UserActivityes.AddAsync(userActivity);
        await _appDbContext.SaveChangesAsync();
    }

    public async Task<GetCheckItemCountDto> GetChecklistInItemCount(Guid ChecklistId)
    {
        return new GetCheckItemCountDto()
        {
            True = _appDbContext.Checkitems.Where(x => x.ChecklistId == ChecklistId && x.Check == true).Count(),
            False = _appDbContext.Checkitems.Where(x => x.ChecklistId == ChecklistId && x.Check == false).Count()
        };
    }

    public async Task RemoveAsync(Guid CheckitemId)
    {
        var checkitem = await _appDbContext.Checkitems.FirstOrDefaultAsync(x => x.Id == CheckitemId);
        if (checkitem is null)
            throw new NotFoundException("Not Found");

        _appDbContext.Checkitems.Remove(checkitem);
        await _appDbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(UpdateCheckitemDto updateCheckitemDto)
    {
        var checkitem = await _appDbContext.Checkitems.Include(x=>x.Checklist).FirstOrDefaultAsync(x => x.Id == updateCheckitemDto.Id);
        if (checkitem is null)
            throw new NotFoundException("Not Found");

        checkitem.Text = updateCheckitemDto.Text;
        checkitem.DueDate = updateCheckitemDto.DueDate;
        checkitem.Check = updateCheckitemDto.Check;

        _appDbContext.Update(checkitem);
        var board = await _appDbContext.Boards.Include(x => x.CardLists)
                   .ThenInclude(x => x.Cards).ThenInclude(x => x.Checklists).ThenInclude(x=>x.Checkitems.Where(x => x.Id == checkitem.Id))
                   .FirstOrDefaultAsync();

        var userActivity = new UserActivity()
        {
            AppUserId = updateCheckitemDto.AppUserId,
            BoardId = board.Id,
            CardId = checkitem.Checklist.CardId,
            ActivityText = $"made changes to {checkitem.Text} checklist"
        };
        await _appDbContext.UserActivityes.AddAsync(userActivity);
        await _appDbContext.SaveChangesAsync();
    }
}
