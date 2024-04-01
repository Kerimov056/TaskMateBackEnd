using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using TaskMate.Service.Abstraction;

namespace TaskMate.Controllers;

[Route("api/[controller]")]
[ApiController]
public class NotifactionsController : ControllerBase
{
    private readonly INotifactionService _actionService;
    public NotifactionsController(INotifactionService actionService)
      =>  _actionService = actionService;

    [HttpPut("[action]")]
    public async Task<IActionResult> SeenUserNotifaction(string AppUserId)
    {
        await _actionService.SeenNotifaction(AppUserId);
        return Ok();
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> UserGetAllNotifactions(string AppUserId)
    {
        return Ok(await _actionService.UserGetAllNotifaction(AppUserId));
    }

}
