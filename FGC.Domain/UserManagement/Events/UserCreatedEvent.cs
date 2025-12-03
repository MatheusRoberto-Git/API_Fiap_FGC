using FGC.Domain.Common.Events;

namespace FGC.Domain.UserManagement.Events
{
    public class UserCreatedEvent : IDomainEvent
    {
        #region [Properties]

        public Guid Id { get; }

        public DateTime OccurredAt { get; }

        public Guid UserId { get; }

        public string Email { get; }

        public string Name { get; }

        public DateTime CreatedAt { get; }

        #endregion

        public UserCreatedEvent(Guid userId, string email, string name, DateTime createdAt)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId não pode ser vazio", nameof(userId));

            Id = Guid.NewGuid();
            OccurredAt = DateTime.UtcNow;
            UserId = userId;
            Email = email ?? throw new ArgumentNullException(nameof(email));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            CreatedAt = createdAt;
        }
    }
}
