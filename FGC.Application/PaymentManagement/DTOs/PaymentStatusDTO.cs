namespace FGC.Application.PaymentManagement.DTOs
{
    public class PaymentStatusDTO
    {
        public Guid PaymentId { get; set; }
        public string TransactionId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime LastUpdated { get; set; }
    }
}
