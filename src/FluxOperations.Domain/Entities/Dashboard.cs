using FluxOperations.Domain.Common;

namespace FluxOperations.Domain.Entities;

public class Dashboard : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string OwnerId { get; private set; } = string.Empty;
    public bool IsDefault { get; private set; }
    public string? Theme { get; private set; }

    private readonly List<Widget> _widgets = [];
    public IReadOnlyCollection<Widget> Widgets => _widgets.AsReadOnly();

    private Dashboard() { }

    public static Dashboard Create(string name, string ownerId, string? description = null, bool isDefault = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(ownerId);

        return new Dashboard
        {
            Name = name,
            OwnerId = ownerId,
            Description = description,
            IsDefault = isDefault,
            Theme = "dark"
        };
    }

    public void AddWidget(Widget widget) => _widgets.Add(widget);

    public void RemoveWidget(Guid widgetId)
    {
        var widget = _widgets.FirstOrDefault(w => w.Id == widgetId)
            ?? throw new InvalidOperationException($"Widget {widgetId} not found on dashboard.");
        _widgets.Remove(widget);
    }

    public void Update(string name, string? description, string? theme)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name;
        Description = description;
        Theme = theme;
        MarkUpdated();
    }
}
