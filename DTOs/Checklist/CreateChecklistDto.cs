﻿namespace TaskMate.DTOs.Checklist;

public class CreateChecklistDto
{
    public string AppUserId { get; set; }
    public string Name { get; set; }
    public Guid CardId { get; set; }
}
