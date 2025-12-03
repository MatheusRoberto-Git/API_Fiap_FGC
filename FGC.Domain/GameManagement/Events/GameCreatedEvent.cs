using FGC.Domain.Common.Events;

namespace FGC.Domain.GameManagement.Events
{
    public class GameCreatedEvent : IDomainEvent
    {
        public Guid Id { get; }
        public DateTime OccurredAt { get; }
        public Guid GameId { get; }
        public string Title { get; }
        public decimal Price { get; }
        public string Category { get; }
        public DateTime CreatedAt { get; }

        public GameCreatedEvent(Guid gameId, string title, decimal price, string category, DateTime createdAt)
        {
            Id = Guid.NewGuid();
            OccurredAt = DateTime.UtcNow;
            GameId = gameId;
            Title = title;
            Price = price;
            Category = category;
            CreatedAt = createdAt;
        }
    }
}
