using TaskMate.DTOs.Boards;
using TaskMate.DTOs.Card;
using TaskMate.DTOs.CardList;
using TaskMate.DTOs.Users;
using TaskMate.DTOs.Workspace;

namespace TaskMate.DTOs;

public class GetGlobalSearch
{
    public List<GetWorkspaceDto>? GetWorkspaceDtos { get; set; }
    public List<GetBoardsDto>? GetBoardsDtos { get; set; }
    public List<GetCardDto>? GetCardDtos { get; set; }
    public List<GetThisCardListDto>? GetCardListDtos { get; set; }
    public List<GetUserDto> GetUserDtos { get; set; }
}
