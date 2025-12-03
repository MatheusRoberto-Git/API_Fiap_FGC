using FGC.Domain.Common.Events;

namespace FGC.Domain.PaymentManagement.Events
{
    public class PaymentCancelledEvent : IDomainEvent
    {
        public Guid Id { get; }
        public DateTime OccurredAt { get; }
        public Guid PaymentId { get; }
        public string TransactionId { get; }
        public DateTime CancelledAt { get; }

        public PaymentCancelledEvent(Guid paymentId, string transactionId, DateTime cancelledAt)
        {
            Id = Guid.NewGuid();
            OccurredAt = DateTime.UtcNow;
            PaymentId = paymentId;
            TransactionId = transactionId;
            CancelledAt = cancelledAt;
        }
    }
}
