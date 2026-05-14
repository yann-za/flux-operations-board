using MediatR;

namespace FluxOperations.Application.Commands.Alerts;

public record ResolveAlertCommand(
    Guid AlertId,
    string ResolvedBy,
    string? ResolutionNote
) : IRequest;
