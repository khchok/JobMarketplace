using FluentValidation;
using JobMarketplace.SharedKernel.Results;
using MediatR;

namespace JobMarketplace.SharedKernel.Behaviours;


public sealed class ValidationBehaviour<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!validators.Any())
            return await next(cancellationToken);

        var context = new ValidationContext<TRequest>(request);
        var failures = validators
            .Select(v => v.Validate(context))
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToList();

        if (failures.Count == 0)
            return await next(cancellationToken);

        var error = Error.Validation(string.Join("; ", failures.Select(f => f.ErrorMessage)));

        // Both Result and Result<T> expose a static Failure(Error) method.
        // We locate it via reflection because TResponse could be either type,
        // and a direct cast from Result to Result<T> fails at runtime.
        var failureMethod = typeof(TResponse).GetMethod(
            "Failure",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static,
            [typeof(Error)])!;

        return (TResponse)failureMethod.Invoke(null, [error])!;
    }
}