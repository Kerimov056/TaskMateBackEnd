namespace TaskMate.DTOs.Card;

public class CreateCardDto
{
    public string AppUserId { get; set; }
    public string Title { get; set; }
    public Guid CardListId { get; set; }
}
