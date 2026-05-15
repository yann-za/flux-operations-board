using FluxOperations.Application.DTOs;
using FluxOperations.Application.Queries.Dashboard;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FluxOperations.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class DashboardController(IMediator mediator) : ControllerBase
{
    /// <summary>Returns aggregated KPI metrics for the operations dashboard.</summary>
    [HttpGet("metrics")]
    [ProducesResponseType(typeof(DashboardMetricsDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMetrics(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetDashboardMetricsQuery(), cancellationToken);
        return Ok(result);
    }
}
