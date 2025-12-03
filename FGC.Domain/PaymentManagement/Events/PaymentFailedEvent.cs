using FGC.Domain.Common.Events;

namespace FGC.Domain.PaymentManagement.Events
{
    public class PaymentFailedEvent : IDomainEvent
    {
        public Guid Id { get; }
        public DateTime OccurredAt { get; }
        public Guid PaymentId { get; }
        public string TransactionId { get; }
        public string FailureReason { get; }
        public DateTime FailedAt { get; }

        public PaymentFailedEvent(Guid paymentId, string transactionId, string failureReason, DateTime failedAt)
        {
            Id = Guid.NewGuid();
            OccurredAt = DateTime.UtcNow;
            PaymentId = paymentId;
            TransactionId = transactionId;
            FailureReason = failureReason;
            FailedAt = failedAt;
        }
    }
}
