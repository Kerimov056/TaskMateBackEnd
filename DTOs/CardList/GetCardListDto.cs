﻿using TaskMate.DTOs.Card;

namespace TaskMate.DTOs.CardList;

public class GetCardListDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public Guid BoardsId { get; set; }
    public ICollection<GetCardDto> getCardDtos { get; set; } = new List<GetCardDto>();

}
