namespace FGC.Application.GameManagement.DTOs
{
    public class SearchGamesDTO
    {
        public string SearchTerm { get; set; } = string.Empty;
        public int? Category { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
    }
}
