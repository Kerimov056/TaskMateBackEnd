using Microsoft.AspNetCore.Mvc;
using TaskMate.Service.Abstraction;

namespace TaskMate.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserActivitysController : ControllerBase
{
    private readonly IUserActivityService _userActivityService;
    public UserActivitysController(IUserActivityService userActivityService)
    => _userActivityService = userActivityService;

    [HttpGet]
    public async Task<IActionResult> GetUserBoardInActivity(string AppUserId, Guid BoardId)
    {
        var allActivity = await _userActivityService.GetUserBoardActivity(AppUserId, BoardId);
        return Ok(allActivity);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetCardInActivity(Guid CardId)
    {
        var allActivity = await _userActivityService.GetCardActivity(CardId);
        return Ok(allActivity);
    }
}
