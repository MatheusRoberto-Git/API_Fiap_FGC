namespace FGC.Application.GameManagement.DTOs
{
    public class UpdateGamePriceDTO
    {
        public Guid GameId { get; set; }
        public decimal NewPrice { get; set; }
    }
}
