using TaskMate.Entities.Common;

namespace TaskMate.Entities;

public class Notification:BaseEntity
{
    public Guid? WorkspaceId { get; set; }
    public Guid? BoardId { get; set; }
    public string? Text { get; set; }
    public bool IsSeen { get; set; } = false;
    //Rellations
    public AppUser AppUser { get; set; }
    public string AppUserId { get; set; }
}
