﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskMate.Context;
using TaskMate.DTOs.Checklist;
using TaskMate.Entities;
using TaskMate.Exceptions;
using TaskMate.Service.Abstraction;

namespace TaskMate.Service.Implementations;

public class ChecklistService : IChecklistService
{
    private readonly AppDbContext _appDbContext;
    private readonly ICheckitemService _checkitemService;
    private readonly IMapper _mapper;
    public ChecklistService(AppDbContext appDbContext, IMapper mapper, ICheckitemService checkitemService)
    {
        _appDbContext = appDbContext;
        _mapper = mapper;
        _checkitemService = checkitemService;

    }
    public async Task CreateAsync([FromForm] CreateChecklistDto createChecklistDto)
    {
        var card = await _appDbContext.Cards.FirstOrDefaultAsync(x => x.Id == createChecklistDto.CardId);
        if (card is null) throw new NotFoundException("Not Found Card");

        var newCheckList = _mapper.Map<Checklist>(createChecklistDto);

        await _appDbContext.Checklists.AddAsync(newCheckList);


        var board = await _appDbContext.Boards.Include(x => x.CardLists)
                    .ThenInclude(x => x.Cards.Where(x => x.Id == card.Id))
                    .FirstOrDefaultAsync();

        var userActivity = new UserActivity()
        {
            AppUserId = createChecklistDto.AppUserId,
            BoardId = board.Id,
            CardId = card.Id,
            ActivityText = $"A new checklist named {createChecklistDto.Name} has been added to the {card.Title} card."
        };
        await _appDbContext.UserActivityes.AddAsync(userActivity);
        await _appDbContext.SaveChangesAsync();

    }

    public async Task<List<GetChecklistDto>> GetAllAsync(Guid CardId)
    {
        var card = await _appDbContext.Cards.FirstOrDefaultAsync(x => x.Id == CardId);
        if (card is null) throw new NotFoundException("Not Found");

        var checkList = await _appDbContext.Checklists.Include(x => x.Checkitems)
                                            .Where(x => x.CardId == CardId).ToListAsync();

        foreach (var item in checkList)
        {
            var checkitemCount = await _checkitemService.GetChecklistInItemCount(item.Id);
            var itemCount = checkitemCount.True + checkitemCount.False;
            if (itemCount != 0)
            {
                decimal percentage = (100 / itemCount) * checkitemCount.True;
                item.CheckPercentage = Convert.ToInt32(Math.Ceiling(percentage));
            }
        }

        return _mapper.Map<List<GetChecklistDto>>(checkList);
    }

    public async Task RemoveAsync(Guid ChecklistId)
    {
        var checklist = await _appDbContext.Checklists.FirstOrDefaultAsync(x => x.Id == ChecklistId);
        if (checklist is null) throw new NotFoundException("Not Found");

        _appDbContext.Checklists.Remove(checklist);
        await _appDbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(UpdateChecklistDto updateChecklistDto)
    {
        var checklist = await _appDbContext.Checklists.FirstOrDefaultAsync(x => x.Id == updateChecklistDto.Id);
        if (checklist is null) throw new NotFoundException("Not Found");

        var oldCheckListName = checklist.Name;
        checklist.Name = updateChecklistDto.Name;

        _appDbContext.Checklists.Update(checklist);


        var board = await _appDbContext.Boards.Include(x => x.CardLists)
                    .ThenInclude(x => x.Cards).ThenInclude(x=>x.Checklists.Where(x=>x.Id==checklist.Id))
                    .FirstOrDefaultAsync();
        var userActivity = new UserActivity()
        {
            AppUserId = updateChecklistDto.AppUserId,
            BoardId = board.Id,
            CardId = checklist.CardId,
            ActivityText = $"Changed the name of the {oldCheckListName} checklist to {checklist.Name}"
        };
        await _appDbContext.UserActivityes.AddAsync(userActivity);
        await _appDbContext.SaveChangesAsync();
    }
}
