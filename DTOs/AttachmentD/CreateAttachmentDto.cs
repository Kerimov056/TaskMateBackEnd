namespace TaskMate.DTOs.AttachmentD;

public class CreateAttachmentDto
{
    public IFormFile? FileName { get; set; }
    public string? Link { get; set; }
    public string? Text { get; set; }
    public Guid CardId { get; set; }
}
