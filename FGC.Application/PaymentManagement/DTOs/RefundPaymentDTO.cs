namespace FGC.Application.PaymentManagement.DTOs
{
    public class RefundPaymentDTO
    {
        public Guid PaymentId { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
