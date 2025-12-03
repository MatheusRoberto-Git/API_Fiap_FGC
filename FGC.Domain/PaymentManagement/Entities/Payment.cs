using FGC.Domain.Common.Entities;
using FGC.Domain.PaymentManagement.Enums;
using FGC.Domain.PaymentManagement.Events;

namespace FGC.Domain.PaymentManagement.Entities
{
    public class Payment : AggregateRoot
    {
        #region [Properties]

        public Guid UserId { get; private set; }

        public Guid GameId { get; private set; }

        public decimal Amount { get; private set; }

        public PaymentStatus Status { get; private set; }

        public PaymentMethod Method { get; private set; }

        public string TransactionId { get; private set; }

        public DateTime CreatedAt { get; private set; }

        public DateTime? ProcessedAt { get; private set; }

        public DateTime? CompletedAt { get; private set; }

        public string FailureReason { get; private set; }

        #endregion

        #region [Constructor]

        private Payment() : base() { }

        private Payment(Guid userId, Guid gameId, decimal amount, PaymentMethod method) : base()
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId é obrigatório", nameof(userId));

            if (gameId == Guid.Empty)
                throw new ArgumentException("GameId é obrigatório", nameof(gameId));

            if (amount <= 0)
                throw new ArgumentException("Valor deve ser maior que zero", nameof(amount));

            UserId = userId;
            GameId = gameId;
            Amount = Math.Round(amount, 2);
            Method = method;
            Status = PaymentStatus.Pending;
            TransactionId = GenerateTransactionId();
            CreatedAt = DateTime.UtcNow;
            FailureReason = string.Empty;
        }

        #endregion

        #region [Factory Methods]

        public static Payment Create(Guid userId, Guid gameId, decimal amount, PaymentMethod method)
        {
            var payment = new Payment(userId, gameId, amount, method);

            payment.AddDomainEvent(new PaymentCreatedEvent(
                payment.Id,
                payment.UserId,
                payment.GameId,
                payment.Amount,
                payment.TransactionId,
                payment.CreatedAt
            ));

            return payment;
        }

        #endregion

        #region [Business Methods]

        public void Process()
        {
            if (Status != PaymentStatus.Pending)
                throw new InvalidOperationException($"Pagamento não pode ser processado. Status atual: {Status}");

            Status = PaymentStatus.Processing;
            ProcessedAt = DateTime.UtcNow;

            AddDomainEvent(new PaymentProcessingEvent(Id, TransactionId, DateTime.UtcNow));
        }

        public void Complete()
        {
            if (Status != PaymentStatus.Processing)
                throw new InvalidOperationException($"Pagamento não pode ser completado. Status atual: {Status}");

            Status = PaymentStatus.Completed;
            CompletedAt = DateTime.UtcNow;

            AddDomainEvent(new PaymentCompletedEvent(
                Id,
                UserId,
                GameId,
                Amount,
                TransactionId,
                DateTime.UtcNow
            ));
        }

        public void Fail(string reason)
        {
            if (Status == PaymentStatus.Completed)
                throw new InvalidOperationException("Pagamento já completado não pode falhar");

            if (Status == PaymentStatus.Refunded)
                throw new InvalidOperationException("Pagamento já reembolsado não pode falhar");

            Status = PaymentStatus.Failed;
            FailureReason = reason ?? "Falha no processamento";

            AddDomainEvent(new PaymentFailedEvent(Id, TransactionId, FailureReason, DateTime.UtcNow));
        }

        public void Refund()
        {
            if (Status != PaymentStatus.Completed)
                throw new InvalidOperationException("Apenas pagamentos completados podem ser reembolsados");

            Status = PaymentStatus.Refunded;

            AddDomainEvent(new PaymentRefundedEvent(Id, UserId, Amount, TransactionId, DateTime.UtcNow));
        }

        public void Cancel()
        {
            if (Status != PaymentStatus.Pending)
                throw new InvalidOperationException("Apenas pagamentos pendentes podem ser cancelados");

            Status = PaymentStatus.Cancelled;

            AddDomainEvent(new PaymentCancelledEvent(Id, TransactionId, DateTime.UtcNow));
        }

        #endregion

        #region [Private Methods]

        private static string GenerateTransactionId()
        {
            return $"TXN-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
        }

        #endregion
    }
}
