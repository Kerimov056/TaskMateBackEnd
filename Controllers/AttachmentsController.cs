using Microsoft.AspNetCore.Mvc;
using System.Net;
using TaskMate.DTOs.AttachmentD;
using TaskMate.Service.Abstraction;

namespace TaskMate.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AttachmentsController : ControllerBase
{
    private readonly IAttachmentService _attachmentService;
    public AttachmentsController(IAttachmentService attachmentService)
    {
        _attachmentService = attachmentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(Guid CardId)
    {
        var boards = await _attachmentService.GetAllAsync(CardId);
        return Ok(boards);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsyncAttachment(CreateAttachmentDto createAttachmentDto)
    {
        await _attachmentService.CreateAsync(createAttachmentDto);
        return StatusCode((int)HttpStatusCode.Created);
    }

}
