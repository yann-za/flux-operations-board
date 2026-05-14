using FluxOperations.Domain.Common;
using FluxOperations.Domain.Enums;

namespace FluxOperations.Domain.Entities;

public class Widget : BaseEntity
{
    public Guid DashboardId { get; private set; }
    public WidgetType Type { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public int PositionX { get; private set; }
    public int PositionY { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }
    public string? ConfigJson { get; private set; }
    public Guid? LinkedFluxId { get; private set; }

    public Dashboard? Dashboard { get; private set; }

    private Widget() { }

    public static Widget Create(
        Guid dashboardId,
        WidgetType type,
        string title,
        int posX,
        int posY,
        int width = 4,
        int height = 3,
        string? configJson = null,
        Guid? linkedFluxId = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);

        return new Widget
        {
            DashboardId = dashboardId,
            Type = type,
            Title = title,
            PositionX = posX,
            PositionY = posY,
            Width = width,
            Height = height,
            ConfigJson = configJson,
            LinkedFluxId = linkedFluxId
        };
    }

    public void UpdatePosition(int posX, int posY, int width, int height)
    {
        PositionX = posX;
        PositionY = posY;
        Width = width;
        Height = height;
        MarkUpdated();
    }
}
