﻿using Microsoft.AspNetCore.Mvc;
using System.Net;
using TaskMate.DTOs.Boards;
using TaskMate.DTOs.CardList;
using TaskMate.Service.Abstraction;
using TaskMate.Service.Implementations;

namespace TaskMate.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CardListsController : ControllerBase
{
    private readonly ICardListService _cardListService;
    public CardListsController(ICardListService cardListService)
       => _cardListService = cardListService;

    //[HttpGet]
    //public async Task<IActionResult> GetAll(Guid WorkspaceId)
    //{
    //    var boards = await _boardsService.GetAllAsync(WorkspaceId);
    //    return Ok(boards);
    //}

    [HttpGet("{BoardId:Guid}")]
    public async Task<IActionResult> GetById(Guid BoardId)
    {
        var byWorkspace = await _cardListService.GetAllCardListAsync(BoardId);
        return Ok(byWorkspace);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCardList([FromForm] CreateCardListDto createCardListDto)
    {
        await _cardListService.CreateAsync(createCardListDto);
        return StatusCode((int)HttpStatusCode.Created);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateCardList([FromForm] UpdateeCardListDto updateeCardListDto)
    {
        await _cardListService.UpdateAsync(updateeCardListDto);
        return Ok();
    }

    [HttpDelete]
    public async Task<IActionResult> Remove(string AdminId, Guid CardListId)
    {
        await _cardListService.Remove(AdminId, CardListId);
        return Ok();
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> AccessUserBoard(string AppUserId, Guid BoardId)
    {
        return Ok(await _cardListService.UserBoardAccess(AppUserId, BoardId));
    }
}
