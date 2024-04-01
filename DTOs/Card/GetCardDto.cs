namespace TaskMate.DTOs.Card;

public class GetCardDto
{
    public Guid Id { get; set; }
    public string? CoverColor { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? Reminder { get; set; }
    public string? DateColor { get; set; }
    public bool? IsDateStatus { get; set; }
    public Guid CardListId { get; set; }
    public DateTime CreatedDate { get; set; }

}
