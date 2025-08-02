namespace FGC.Domain.Common.Events
{
    public class AdminUserCreatedEvent : IDomainEvent
    {
        #region [Properties]

        public Guid Id { get; }

        public DateTime OccurredAt { get; }

        public Guid NewAdminId { get; }

        public string Email { get; }

        public string Name { get; }

        public Guid CreatedByAdminId { get; }

        public DateTime CreatedAt { get; }

        #endregion

        public AdminUserCreatedEvent(Guid newAdminId, string email, string name, Guid createdByAdminId, DateTime createdAt)
        {
            if (newAdminId == Guid.Empty)
                throw new ArgumentException("NewAdminId não pode ser vazio", nameof(newAdminId));

            if (createdByAdminId == Guid.Empty)
                throw new ArgumentException("CreatedByAdminId não pode ser vazio", nameof(createdByAdminId));

            Id = Guid.NewGuid();
            OccurredAt = DateTime.UtcNow;
            NewAdminId = newAdminId;
            Email = email ?? throw new ArgumentNullException(nameof(email));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            CreatedByAdminId = createdByAdminId;
            CreatedAt = createdAt;
        }
    }
}
