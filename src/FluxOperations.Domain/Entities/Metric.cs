using FluxOperations.Domain.Common;

namespace FluxOperations.Domain.Entities;

public class Metric : BaseEntity
{
    public Guid FluxId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public double Value { get; private set; }
    public string Unit { get; private set; } = string.Empty;
    public DateTime RecordedAt { get; private set; }

    public Flux? Flux { get; private set; }

    private Metric() { }

    public static Metric Record(Guid fluxId, string name, double value, string unit)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(unit);

        return new Metric
        {
            FluxId = fluxId,
            Name = name,
            Value = value,
            Unit = unit,
            RecordedAt = DateTime.UtcNow
        };
    }
}
