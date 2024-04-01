namespace TaskMate.DTOs.Checklist;

public class UpdateChecklistDto
{
    public string AppUserId { get; set; }
    public Guid Id { get; set; }
    public string Name { get; set; }
}
