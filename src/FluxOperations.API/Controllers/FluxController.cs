using FluxOperations.Application.Commands.Fluxes;
using FluxOperations.Application.DTOs;
using FluxOperations.Application.Queries.Fluxes;
using FluxOperations.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FluxOperations.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class FluxController(IMediator mediator) : ControllerBase
{
    /// <summary>Returns a paginated, filtered list of flux operations.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<FluxDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] FluxStatus? status = null,
        [FromQuery] FluxType? type = null,
        [FromQuery] string? search = null,
        [FromQuery] bool includeArchived = false,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(
            new GetAllFluxesQuery(pageNumber, pageSize, status, type, search, includeArchived),
            cancellationToken);
        return Ok(result);
    }

    /// <summary>Returns a single flux operation by its identifier.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(FluxDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetFluxByIdQuery(id), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>Creates a new flux operation.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(FluxDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateFluxCommand command,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>Updates an existing flux operation.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(FluxDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateFluxCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.Id)
            return BadRequest("Route id and body id mismatch.");

        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>Archives (soft-deletes) a flux operation.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteFluxCommand(id), cancellationToken);
        return NoContent();
    }

    /// <summary>Pauses an active flux operation.</summary>
    [HttpPost("{id:guid}/pause")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Pause(Guid id, CancellationToken cancellationToken)
    {
        await mediator.Send(new PauseFluxCommand(id), cancellationToken);
        return NoContent();
    }

    /// <summary>Resumes a paused flux operation.</summary>
    [HttpPost("{id:guid}/resume")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Resume(Guid id, CancellationToken cancellationToken)
    {
        await mediator.Send(new ResumeFluxCommand(id), cancellationToken);
        return NoContent();
    }
}
