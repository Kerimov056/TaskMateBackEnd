using TaskMate.Entities;

namespace TaskMate.DTOs.UserActivityD;

public class GetUserActivityDto
{
    public Guid Id { get; set; } 
    public DateTime CreatedDate { get; set; }
    public string ActivityText { get; set; }
    public Guid? BoardId { get; set; }
    public Guid? CardId { get; set; }
    public string UserName { get; set; }
    public string AppUserId { get; set; }
}
