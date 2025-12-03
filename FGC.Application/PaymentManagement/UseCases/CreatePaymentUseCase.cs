using FGC.Application.PaymentManagement.DTOs;
using FGC.Domain.PaymentManagement.Entities;
using FGC.Domain.PaymentManagement.Enums;
using FGC.Domain.PaymentManagement.Interfaces;

namespace FGC.Application.PaymentManagement.UseCases
{
    public class CreatePaymentUseCase
    {
        private readonly IPaymentRepository _paymentRepository;

        public CreatePaymentUseCase(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
        }

        public async Task<PaymentResponseDTO> ExecuteAsync(CreatePaymentDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var method = (PaymentMethod)dto.PaymentMethod;

            var payment = Payment.Create(dto.UserId, dto.GameId, dto.Amount, method);

            await _paymentRepository.SaveAsync(payment);
            await ProcessDomainEventsAsync(payment);
            payment.ClearDomainEvents();

            return MapToResponseDto(payment);
        }

        private async Task ProcessDomainEventsAsync(Payment payment)
        {
            foreach (var domainEvent in payment.DomainEvents)
            {
                Console.WriteLine($"[EVENT] {domainEvent.GetType().Name} - Payment: {payment.TransactionId}");
            }
        }

        private static PaymentResponseDTO MapToResponseDto(Payment payment)
        {
            return new PaymentResponseDTO
            {
                Id = payment.Id,
                UserId = payment.UserId,
                GameId = payment.GameId,
                Amount = payment.Amount,
                Status = payment.Status.ToString(),
                Method = payment.Method.ToString(),
                TransactionId = payment.TransactionId,
                CreatedAt = payment.CreatedAt,
                ProcessedAt = payment.ProcessedAt,
                CompletedAt = payment.CompletedAt,
                FailureReason = payment.FailureReason
            };
        }
    }
}
