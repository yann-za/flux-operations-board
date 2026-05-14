using FluxOperations.Domain.Common;
using FluxOperations.Domain.Enums;

namespace FluxOperations.Domain.Entities;

public class Alert : BaseEntity
{
    public Guid FluxId { get; private set; }
    public AlertSeverity Severity { get; private set; }
    public string Message { get; private set; } = string.Empty;
    public bool IsResolved { get; private set; }
    public DateTime? ResolvedAt { get; private set; }
    public string? ResolvedBy { get; private set; }
    public string? ResolutionNote { get; private set; }

    public Flux? Flux { get; private set; }

    private Alert() { }

    public static Alert Create(Guid fluxId, AlertSeverity severity, string message, string? createdBy = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(message);

        return new Alert
        {
            FluxId = fluxId,
            Severity = severity,
            Message = message,
            IsResolved = false,
            CreatedBy = createdBy
        };
    }

    public void Resolve(string resolvedBy, string? resolutionNote = null)
    {
        if (IsResolved)
            throw new InvalidOperationException("Alert is already resolved.");

        IsResolved = true;
        ResolvedAt = DateTime.UtcNow;
        ResolvedBy = resolvedBy;
        ResolutionNote = resolutionNote;
        MarkUpdated(resolvedBy);
    }
}
