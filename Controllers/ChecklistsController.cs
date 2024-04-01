﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using TaskMate.DTOs.Checkitem;
using TaskMate.DTOs.Checklist;
using TaskMate.Service.Abstraction;
using TaskMate.Service.Implementations;

namespace TaskMate.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ChecklistsController : ControllerBase
{
    private readonly IChecklistService _checklistService;
    public ChecklistsController(IChecklistService checklistService)
        => _checklistService = checklistService;


    [HttpGet("[action]")]
    public async Task<IActionResult> GetAll(Guid CardId)
    {
        var boards = await _checklistService.GetAllAsync(CardId);
        return Ok(boards);
    }

    [HttpPost]
    public async Task<IActionResult> Createchecklist([FromForm] CreateChecklistDto createChecklistDto)
    {
        await _checklistService.CreateAsync(createChecklistDto);
        return StatusCode((int)HttpStatusCode.Created);
    }
    [HttpPut]
    public async Task<IActionResult> UpdateCheckItem([FromForm] UpdateChecklistDto updateChecklistDto)
    {
        await _checklistService.UpdateAsync(updateChecklistDto);
        return Ok();
    }

    [HttpDelete]
    public async Task<IActionResult> Remove(Guid CheckListId)
    {
        await _checklistService.RemoveAsync(CheckListId);
        return Ok();
    }
}
