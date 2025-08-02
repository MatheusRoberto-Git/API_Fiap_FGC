namespace FGC.Domain.Common.Events
{
    public class UserAuthenticatedEvent : IDomainEvent
    {
        #region [Properties]

        public Guid Id { get; }

        public DateTime OccurredAt { get; }

        public Guid UserId { get; }

        public string Email { get; }

        public DateTime LoginAt { get; }

        #endregion

        public UserAuthenticatedEvent(Guid userId, string email, DateTime loginAt)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId não pode ser vazio", nameof(userId));

            Id = Guid.NewGuid();
            OccurredAt = DateTime.UtcNow;
            UserId = userId;
            Email = email ?? throw new ArgumentNullException(nameof(email));
            LoginAt = loginAt;
        }
    }
}
