using FluxOperations.Domain.Common;

namespace FluxOperations.Domain.Events;

public sealed class FluxCreatedEvent(Guid fluxId, string fluxName) : BaseDomainEvent
{
    public Guid FluxId { get; } = fluxId;
    public string FluxName { get; } = fluxName;
}
