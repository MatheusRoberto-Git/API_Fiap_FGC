using FGC.Application.PaymentManagement.DTOs;
using FGC.Domain.PaymentManagement.Entities;
using FGC.Domain.PaymentManagement.Interfaces;

namespace FGC.Application.PaymentManagement.UseCases
{
    public class ProcessPaymentUseCase
    {
        private readonly IPaymentRepository _paymentRepository;

        public ProcessPaymentUseCase(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
        }

        public async Task<PaymentResponseDTO> ExecuteAsync(ProcessPaymentDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var payment = await _paymentRepository.GetByIdAsync(dto.PaymentId);

            if (payment == null)
                throw new InvalidOperationException($"Pagamento com ID {dto.PaymentId} não encontrado");

            // Simula processamento
            payment.Process();

            // Simula sucesso (em produção seria integração com gateway)
            var random = new Random();
            var success = random.Next(100) < 90; // 90% de sucesso

            if (success)
            {
                payment.Complete();
            }
            else
            {
                payment.Fail("Cartão recusado pela operadora");
            }

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
