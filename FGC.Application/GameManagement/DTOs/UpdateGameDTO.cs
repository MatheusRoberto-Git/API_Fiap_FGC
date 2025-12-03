namespace FGC.Application.GameManagement.DTOs
{
    public class UpdateGameDTO
    {
        public Guid GameId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
