using FGC.Application.PaymentManagement.DTOs;
using FGC.Domain.PaymentManagement.Interfaces;

namespace FGC.Application.PaymentManagement.UseCases
{
    public class GetPaymentStatusUseCase
    {
        private readonly IPaymentRepository _paymentRepository;

        public GetPaymentStatusUseCase(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
        }

        public async Task<PaymentStatusDTO> ExecuteAsync(Guid paymentId)
        {
            if (paymentId == Guid.Empty)
                throw new ArgumentException("PaymentId é obrigatório");

            var payment = await _paymentRepository.GetByIdAsync(paymentId);

            if (payment == null)
                throw new InvalidOperationException($"Pagamento com ID {paymentId} não encontrado");

            return new PaymentStatusDTO
            {
                PaymentId = payment.Id,
                TransactionId = payment.TransactionId,
                Status = payment.Status.ToString(),
                LastUpdated = payment.CompletedAt ?? payment.ProcessedAt ?? payment.CreatedAt
            };
        }
    }
}
