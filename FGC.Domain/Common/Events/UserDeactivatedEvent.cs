namespace FGC.Domain.Common.Events
{
    public class UserDeactivatedEvent : IDomainEvent
    {
        #region [Properties]

        public Guid Id { get; }
        public DateTime OccurredAt { get; }
        public Guid UserId { get; }
        public string Email { get; }
        public DateTime DeactivatedAt { get; }

        #endregion

        public UserDeactivatedEvent(Guid userId, string email, DateTime deactivatedAt)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId não pode ser vazio", nameof(userId));

            Id = Guid.NewGuid();
            OccurredAt = DateTime.UtcNow;
            UserId = userId;
            Email = email ?? throw new ArgumentNullException(nameof(email));
            DeactivatedAt = deactivatedAt;
        }
    }
}
