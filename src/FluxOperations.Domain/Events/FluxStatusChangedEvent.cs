using FluxOperations.Domain.Common;
using FluxOperations.Domain.Enums;

namespace FluxOperations.Domain.Events;

public sealed class FluxStatusChangedEvent(Guid fluxId, FluxStatus newStatus) : BaseDomainEvent
{
    public Guid FluxId { get; } = fluxId;
    public FluxStatus NewStatus { get; } = newStatus;
}
