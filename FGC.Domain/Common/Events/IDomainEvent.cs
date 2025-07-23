using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FGC.Domain.Common.Events
{
    public interface IDomainEvent
    {
        Guid Id { get; }

        DateTime OccurredAt { get; }
    }
}
