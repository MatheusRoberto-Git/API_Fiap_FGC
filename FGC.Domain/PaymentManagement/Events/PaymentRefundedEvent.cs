using FGC.Domain.Common.Events;

namespace FGC.Domain.PaymentManagement.Events
{
    public class PaymentRefundedEvent : IDomainEvent
    {
        public Guid Id { get; }
        public DateTime OccurredAt { get; }
        public Guid PaymentId { get; }
        public Guid UserId { get; }
        public decimal Amount { get; }
        public string TransactionId { get; }
        public DateTime RefundedAt { get; }

        public PaymentRefundedEvent(Guid paymentId, Guid userId, decimal amount, string transactionId, DateTime refundedAt)
        {
            Id = Guid.NewGuid();
            OccurredAt = DateTime.UtcNow;
            PaymentId = paymentId;
            UserId = userId;
            Amount = amount;
            TransactionId = transactionId;
            RefundedAt = refundedAt;
        }
    }
}
