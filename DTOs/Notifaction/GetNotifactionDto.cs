namespace TaskMate.DTOs.Notifaction;

public class GetNotifactionDto
{
    public Guid? WorkspaceId { get; set; }
    public Guid? BoardId { get; set; }
    public string? Text { get; set; }
    public bool IsSeen { get; set; } = false;
    public string AppUserId { get; set; }
}
