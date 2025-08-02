namespace FGC.Domain.Common.Events
{
    public class UserReactivatedEvent : IDomainEvent
    {
        #region [Properties]

        public Guid Id { get; }

        public DateTime OccurredAt { get; }

        public Guid UserId { get; }

        public string Email { get; }

        public DateTime ReactivatedAt { get; }

        #endregion

        public UserReactivatedEvent(Guid userId, string email, DateTime reactivatedAt)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId não pode ser vazio", nameof(userId));

            Id = Guid.NewGuid();
            OccurredAt = DateTime.UtcNow;
            UserId = userId;
            Email = email ?? throw new ArgumentNullException(nameof(email));
            ReactivatedAt = reactivatedAt;
        }
    }
}
