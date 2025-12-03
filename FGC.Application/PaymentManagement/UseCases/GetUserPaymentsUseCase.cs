using FGC.Application.PaymentManagement.DTOs;
using FGC.Domain.PaymentManagement.Entities;
using FGC.Domain.PaymentManagement.Interfaces;

namespace FGC.Application.PaymentManagement.UseCases
{
    public class GetUserPaymentsUseCase
    {
        private readonly IPaymentRepository _paymentRepository;

        public GetUserPaymentsUseCase(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
        }

        public async Task<IEnumerable<PaymentResponseDTO>> ExecuteAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId é obrigatório");

            var payments = await _paymentRepository.GetByUserIdAsync(userId);

            return payments.Select(MapToResponseDto);
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
