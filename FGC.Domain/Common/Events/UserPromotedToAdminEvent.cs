namespace FGC.Domain.Common.Events
{
    public class UserPromotedToAdminEvent : IDomainEvent
    {
        #region [Properties]

        public Guid Id { get; }
        public DateTime OccurredAt { get; }
        public Guid UserId { get; }
        public string Email { get; }
        public string Name { get; }
        public DateTime PromotedAt { get; }

        #endregion

        public UserPromotedToAdminEvent(Guid userId, string email, string name, DateTime promotedAt)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId não pode ser vazio", nameof(userId));

            Id = Guid.NewGuid();
            OccurredAt = DateTime.UtcNow;
            UserId = userId;
            Email = email ?? throw new ArgumentNullException(nameof(email));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            PromotedAt = promotedAt;
        }
    }
}
