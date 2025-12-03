using FGC.Domain.Common.Events;

namespace FGC.Domain.GameManagement.Events
{
    public class GameDeactivatedEvent : IDomainEvent
    {
        public Guid Id { get; }
        public DateTime OccurredAt { get; }
        public Guid GameId { get; }
        public string Title { get; }
        public DateTime DeactivatedAt { get; }

        public GameDeactivatedEvent(Guid gameId, string title, DateTime deactivatedAt)
        {
            Id = Guid.NewGuid();
            OccurredAt = DateTime.UtcNow;
            GameId = gameId;
            Title = title;
            DeactivatedAt = deactivatedAt;
        }
    }
}
