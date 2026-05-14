using MediatR;

namespace FluxOperations.Domain.Common;

public abstract class BaseDomainEvent : INotification
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}
