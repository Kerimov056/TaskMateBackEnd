using TaskMate.DTOs.Card;
using TaskMate.Entities;

namespace TaskMate.Service.Abstraction;

public interface ICardService
{
    Task CreateAsync(CreateCardDto createCardDto);
    Task<GetCardDto> GetByIdAsync(Guid CardId);
    Task Remove(string AppUserId, Guid CardId);
    Task UpdateAsync(UpdateCardDto updateCardDto);
    Task AddCardDateAsync(CardAddDatesDto cardAddDatesDto);
    Task MoveCardAsync(MoveCard moveCard);
    Task DragAndDrop(DragAndDropCardDto dragAndDropCardDto);
    Task DeleteCardDate(Guid CardId);
    Task CardDateEditIsStatus(Guid CardId);
    Task<List<AppUser>> GetAllUsers(Guid boardId);
}
