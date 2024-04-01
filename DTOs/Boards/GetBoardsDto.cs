using TaskMate.DTOs.CardList;
using TaskMate.Helper.Enum.Board;

namespace TaskMate.DTOs.Boards;

public class GetBoardsDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public BoardAccessibility BoardAccessibility { get; set; } 
    public Guid WorkspaceId { get; set; }
    public DateTime CreatedDate { get; set; }
    public bool IsArchive { get; set; }
    public ICollection<GetCardListDto> getCardListDtos { get; set; } = new List<GetCardListDto>();
}
