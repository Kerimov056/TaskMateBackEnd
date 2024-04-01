using TaskMate.Entities.Common;

namespace TaskMate.Entities;

public class UserActivity:BaseEntity
{
    public string ActivityText { get; set; }
    public Guid? BoardId { get; set; }
    public Guid? CardId { get; set; }
    public AppUser AppUser { get; set; } 
    public string AppUserId { get; set; }
}
