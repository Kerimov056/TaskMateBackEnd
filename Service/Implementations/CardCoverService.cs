using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TaskMate.Context;
using TaskMate.DTOs;
using TaskMate.DTOs.CardCoverDto;
using TaskMate.Entities;
using TaskMate.Exceptions;
using TaskMate.Service.Abstraction;

namespace TaskMate.Service.Implementations
{
    public class CardCoverService : ICardCoverService
    {
        public readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public CardCoverService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task AddOrUpdateCardCover(CardCoverCreateDto Dto)
        {
            var card = await _context.Cards.FirstOrDefaultAsync(x => x.Id == Dto.CardId);
            if (card is null) throw new NotFoundException("Not Found");
            card.CoverColor = Dto.Color;
            await _context.SaveChangesAsync();
        }

        public async Task<string> GetCardCover(GetCardCoverDto Dto)
        {
            var card = await _context.Cards.FirstOrDefaultAsync(x => x.Id == Dto.CardId);
            if (card is null) throw new NotFoundException("Not Found");
            else return card.CoverColor;
        }
    }
}
