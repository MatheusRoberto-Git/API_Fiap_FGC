using FGC.Domain.Common.Events;

namespace FGC.Domain.PaymentManagement.Events
{
    public class PaymentCreatedEvent : IDomainEvent
    {
        public Guid Id { get; }
        public DateTime OccurredAt { get; }
        public Guid PaymentId { get; }
        public Guid UserId { get; }
        public Guid GameId { get; }
        public decimal Amount { get; }
        public string TransactionId { get; }
        public DateTime CreatedAt { get; }

        public PaymentCreatedEvent(Guid paymentId, Guid userId, Guid gameId, decimal amount, string transactionId, DateTime createdAt)
        {
            Id = Guid.NewGuid();
            OccurredAt = DateTime.UtcNow;
            PaymentId = paymentId;
            UserId = userId;
            GameId = gameId;
            Amount = amount;
            TransactionId = transactionId;
            CreatedAt = createdAt;
        }
    }
}
