using FluxOperations.Application.Commands.Alerts;
using FluxOperations.Application.DTOs;
using FluxOperations.Application.Queries.Alerts;
using FluxOperations.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FluxOperations.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AlertController(IMediator mediator) : ControllerBase
{
    /// <summary>Returns active (unresolved) alerts, optionally filtered by flux or severity.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AlertDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActive(
        [FromQuery] Guid? fluxId = null,
        [FromQuery] AlertSeverity? severity = null,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetActiveAlertsQuery(fluxId, severity), cancellationToken);
        return Ok(result);
    }

    /// <summary>Creates a manual alert on a flux operation.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(AlertDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create(
        [FromBody] CreateAlertCommand command,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return Created(string.Empty, result);
    }

    /// <summary>Resolves an active alert.</summary>
    [HttpPost("{id:guid}/resolve")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Resolve(
        Guid id,
        [FromBody] ResolveAlertRequest request,
        CancellationToken cancellationToken)
    {
        await mediator.Send(
            new ResolveAlertCommand(id, request.ResolvedBy, request.ResolutionNote),
            cancellationToken);
        return NoContent();
    }
}

public record ResolveAlertRequest(string ResolvedBy, string? ResolutionNote);
