namespace TaskMate.DTOs
{
    public class GetCardCoverDto
    {
        public Guid? WorkspaceId { get; set; }
        public Guid? BoardId { get; set; }
        public Guid? CardId { get; set; }
    }
}
