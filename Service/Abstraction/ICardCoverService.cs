using Humanizer;
using TaskMate.DTOs;
using TaskMate.DTOs.CardCoverDto;

namespace TaskMate.Service.Abstraction
{
    public interface ICardCoverService
    {
        Task AddOrUpdateCardCover(CardCoverCreateDto Dto);
        Task<string> GetCardCover(GetCardCoverDto Dto);
    }
}
