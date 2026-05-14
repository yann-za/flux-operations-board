using MediatR;
using Microsoft.Extensions.Logging;

namespace FluxOperations.Application.Common.Behaviours;

public sealed class LoggingBehaviour<TRequest, TResponse>(ILogger<LoggingBehaviour<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        logger.LogInformation("FluxOps Request: {Name} {@Request}", requestName, request);

        var response = await next();

        logger.LogInformation("FluxOps Response: {Name} {@Response}", requestName, response);
        return response;
    }
}
