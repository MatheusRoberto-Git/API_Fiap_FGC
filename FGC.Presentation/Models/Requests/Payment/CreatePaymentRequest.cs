namespace FGC.Presentation.Models.Requests.Payment
{
    public class CreatePaymentRequest
    {
        public Guid UserId { get; set; }
        public Guid GameId { get; set; }
        public decimal Amount { get; set; }
        public int PaymentMethod { get; set; }
    }
}
