﻿namespace TaskMate.DTOs.Workspace;

public class AddUserWorkspace
{
    public string AdminId { get; set; }
    public Guid WorkspaceId { get; set; }
    public string AppUserId { get; set; }
}
