using TaskMate.DTOs.Boards;
using TaskMate.DTOs.CardList;

namespace TaskMate.DTOs.Workspace;

public class GetWorkspaceInBoardDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public bool IsArchive { get; set; } = false;
    public ICollection<GetBoardsDto> getBoardsDtos { get; set; } = new List<GetBoardsDto>();
}
