using Microsoft.AspNetCore.Mvc;
using TaskMate.DTOs;
using TaskMate.DTOs.CardCoverDto;
using TaskMate.DTOs.Comment;
using TaskMate.DTOs.Users;
using TaskMate.Helper;
using TaskMate.Service.Abstraction;
using TaskMate.Service.Implementations;

namespace TaskMate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CardCoverController : ControllerBase
    {
        private readonly ICardCoverService _cardCoverService;

        public CardCoverController(ICardCoverService cardCoverService)
        {
            _cardCoverService = cardCoverService;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> CreateCardCover(CardCoverCreateDto Dto)
        {
            await _cardCoverService.AddOrUpdateCardCover(Dto);
            return Ok();
        }
        [HttpGet("[action]")]
        public async Task<IActionResult> GetCardCover(Guid CardId)
        {
            GetCardCoverDto getCardCoverDto = new GetCardCoverDto();
            getCardCoverDto.CardId = CardId;
            var Cover = await _cardCoverService.GetCardCover(getCardCoverDto);
            return Ok(Cover);
        }

    }
}
