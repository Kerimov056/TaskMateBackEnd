using TaskMate.Helper.Enum.Board;

namespace TaskMate.DTOs.Boards;

public class CreateBoardsDto
{
    public string AppUserId { get; set; }
    public string Title { get; set; }
    public BoardAccessibility? BoardAccessibility { get; set; }
    public Guid WorkspaceId { get; set; }
}
