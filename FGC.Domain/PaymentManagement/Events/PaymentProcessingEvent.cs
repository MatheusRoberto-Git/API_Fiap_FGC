using FGC.Domain.Common.Events;

namespace FGC.Domain.PaymentManagement.Events
{
    public class PaymentProcessingEvent : IDomainEvent
    {
        public Guid Id { get; }
        public DateTime OccurredAt { get; }
        public Guid PaymentId { get; }
        public string TransactionId { get; }
        public DateTime ProcessedAt { get; }

        public PaymentProcessingEvent(Guid paymentId, string transactionId, DateTime processedAt)
        {
            Id = Guid.NewGuid();
            OccurredAt = DateTime.UtcNow;
            PaymentId = paymentId;
            TransactionId = transactionId;
            ProcessedAt = processedAt;
        }
    }
}
