using FGC.Domain.Common.Events;

namespace FGC.Domain.GameManagement.Events
{
    public class GamePriceUpdatedEvent : IDomainEvent
    {
        public Guid Id { get; }
        public DateTime OccurredAt { get; }
        public Guid GameId { get; }
        public string Title { get; }
        public decimal OldPrice { get; }
        public decimal NewPrice { get; }
        public DateTime UpdatedAt { get; }

        public GamePriceUpdatedEvent(Guid gameId, string title, decimal oldPrice, decimal newPrice, DateTime updatedAt)
        {
            Id = Guid.NewGuid();
            OccurredAt = DateTime.UtcNow;
            GameId = gameId;
            Title = title;
            OldPrice = oldPrice;
            NewPrice = newPrice;
            UpdatedAt = updatedAt;
        }
    }
}
