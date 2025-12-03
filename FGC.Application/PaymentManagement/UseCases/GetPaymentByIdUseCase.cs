using FGC.Application.PaymentManagement.DTOs;
using FGC.Domain.PaymentManagement.Entities;
using FGC.Domain.PaymentManagement.Interfaces;

namespace FGC.Application.PaymentManagement.UseCases
{
    public class GetPaymentByIdUseCase
    {
        private readonly IPaymentRepository _paymentRepository;

        public GetPaymentByIdUseCase(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
        }

        public async Task<PaymentResponseDTO> ExecuteAsync(Guid paymentId)
        {
            if (paymentId == Guid.Empty)
                throw new ArgumentException("PaymentId é obrigatório");

            var payment = await _paymentRepository.GetByIdAsync(paymentId);

            if (payment == null)
                throw new InvalidOperationException($"Pagamento com ID {paymentId} não encontrado");

            return MapToResponseDto(payment);
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
