namespace TaskMate.DTOs.CardList;

public class GetThisCardListDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public Guid BoardsId { get; set; }
    public DateTime CreatedDate { get; set; }
}
