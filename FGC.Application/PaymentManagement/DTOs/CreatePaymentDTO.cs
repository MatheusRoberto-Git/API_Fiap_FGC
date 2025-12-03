namespace FGC.Application.PaymentManagement.DTOs
{
    public class CreatePaymentDTO
    {
        public Guid UserId { get; set; }
        public Guid GameId { get; set; }
        public decimal Amount { get; set; }
        public int PaymentMethod { get; set; }
    }
}
