using JobMarketplace.SharedKernel.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace JobMarketplace.SharedKernel.Behaviours;


public sealed class LoggingBehaviour<TRequest, TResponse>(
    ILogger<LoggingBehaviour<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        logger.LogInformation("Handling {RequestName}", requestName);

        var response = await next(cancellationToken);

        if (response.IsFailure)
            logger.LogWarning("Request {RequestName} failed: {Error}", requestName, response.Error.Description);
        else
            logger.LogInformation("Request {RequestName} succeeded", requestName);

        return response;
    }
}