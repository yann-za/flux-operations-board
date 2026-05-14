using FluxOperations.Domain.Common;
using FluxOperations.Domain.Enums;
using FluxOperations.Domain.Events;

namespace FluxOperations.Domain.Entities;

public class Flux : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public FluxType Type { get; private set; }
    public FluxStatus Status { get; private set; }
    public string? SourceSystem { get; private set; }
    public string? TargetSystem { get; private set; }
    public string? ScheduleCron { get; private set; }
    public DateTime? LastExecutedAt { get; private set; }
    public DateTime? NextExecutionAt { get; private set; }
    public long? ThroughputPerHour { get; private set; }
    public double? ErrorRatePercent { get; private set; }
    public bool IsArchived { get; private set; }

    private readonly List<Alert> _alerts = [];
    public IReadOnlyCollection<Alert> Alerts => _alerts.AsReadOnly();

    private readonly List<Metric> _metrics = [];
    public IReadOnlyCollection<Metric> Metrics => _metrics.AsReadOnly();

    private Flux() { }

    public static Flux Create(
        string name,
        FluxType type,
        string? description = null,
        string? sourceSystem = null,
        string? targetSystem = null,
        string? scheduleCron = null,
        string? createdBy = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var flux = new Flux
        {
            Name = name,
            Type = type,
            Description = description,
            SourceSystem = sourceSystem,
            TargetSystem = targetSystem,
            ScheduleCron = scheduleCron,
            Status = FluxStatus.Inactive,
            CreatedBy = createdBy
        };

        flux.AddDomainEvent(new FluxCreatedEvent(flux.Id, flux.Name));
        return flux;
    }

    public void Activate()
    {
        Status = FluxStatus.Active;
        MarkUpdated();
        AddDomainEvent(new FluxStatusChangedEvent(Id, FluxStatus.Active));
    }

    public void Pause()
    {
        if (Status != FluxStatus.Active)
            throw new InvalidOperationException($"Cannot pause a flux with status {Status}.");

        Status = FluxStatus.Paused;
        MarkUpdated();
        AddDomainEvent(new FluxStatusChangedEvent(Id, FluxStatus.Paused));
    }

    public void Resume()
    {
        if (Status != FluxStatus.Paused)
            throw new InvalidOperationException($"Cannot resume a flux with status {Status}.");

        Status = FluxStatus.Active;
        MarkUpdated();
        AddDomainEvent(new FluxStatusChangedEvent(Id, FluxStatus.Active));
    }

    public void MarkAsError(string reason)
    {
        Status = FluxStatus.Error;
        MarkUpdated();
        AddDomainEvent(new FluxStatusChangedEvent(Id, FluxStatus.Error));

        var alert = Alert.Create(Id, AlertSeverity.Critical, $"Flux error: {reason}");
        _alerts.Add(alert);
    }

    public void Update(
        string name,
        string? description,
        string? sourceSystem,
        string? targetSystem,
        string? scheduleCron,
        string? updatedBy = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name;
        Description = description;
        SourceSystem = sourceSystem;
        TargetSystem = targetSystem;
        ScheduleCron = scheduleCron;
        MarkUpdated(updatedBy);
    }

    public void RecordExecution(long throughputPerHour, double errorRatePercent)
    {
        LastExecutedAt = DateTime.UtcNow;
        ThroughputPerHour = throughputPerHour;
        ErrorRatePercent = errorRatePercent;

        if (errorRatePercent > 5.0 && Status == FluxStatus.Active)
        {
            Status = FluxStatus.Warning;
            AddDomainEvent(new FluxStatusChangedEvent(Id, FluxStatus.Warning));
        }

        MarkUpdated();
    }

    public void Archive() => IsArchived = true;
}
