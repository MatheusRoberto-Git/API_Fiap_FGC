using FGC.Domain.Common.Events;

namespace FGC.Domain.UserManagement.Events
{
    public class PasswordChangedEvent : IDomainEvent
    {
        #region [Properties]

        public Guid Id { get; }

        public DateTime OccurredAt { get; }

        public Guid UserId { get; }

        public DateTime ChangedAt { get; }

        #endregion

        public PasswordChangedEvent(Guid userId, DateTime changedAt)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId não pode ser vazio", nameof(userId));

            Id = Guid.NewGuid();
            OccurredAt = DateTime.UtcNow;
            UserId = userId;
            ChangedAt = changedAt;
        }
    }
}