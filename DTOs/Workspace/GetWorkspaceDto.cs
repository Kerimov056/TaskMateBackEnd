﻿namespace TaskMate.DTOs.Workspace;

public class GetWorkspaceDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Type { get; set; }
    public DateTime CreatedDate { get; set; }
    public bool IsWorkspace { get; set; } = true;
    public bool IsArchive { get; set; } = false;

}
