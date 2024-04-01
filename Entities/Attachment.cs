using TaskMate.Entities.Common;

namespace TaskMate.Entities;

public class Attachment: BaseEntity
{
    public string? FileName { get; set; }
    public string? Link { get; set; }
    public string? Text { get; set; }

    //Rellations
    public Card Cards { get; set; }
    public Guid CardsId { get; set; }
}
