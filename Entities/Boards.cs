using TaskMate.Entities.Common;
using TaskMate.Helper.Enum.Board;

namespace TaskMate.Entities;

public class Boards : BaseEntity
{
    public string Title { get; set; }
    public BoardAccessibility BoardAccessibility { get; set; } = BoardAccessibility.Public;
    //Rellations
    public List<CardList>? CardLists { get; set; }
    public Workspace Workspace { get; set; }
    public Guid WorkspaceId { get; set; }
    public List<UserBoards>? UserBoards { get; set; }
    public List<Labels>? Labels { get; set; }

}