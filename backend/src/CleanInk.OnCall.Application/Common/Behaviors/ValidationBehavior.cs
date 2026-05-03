using CleanInk.OnCall.Shared;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanInk.OnCall.Application.Common.Behaviors;

/// <summary>
/// MediatR pipeline behavior that runs FluentValidation validators before the handler.
/// Returns a failure <see cref="Result"/> if any validation errors are found.
/// </summary>
/// <typeparam name="TRequest">The MediatR request type.</typeparam>
/// <typeparam name="TResponse">The MediatR response type, must be a <see cref="Result"/>.</typeparam>
public sealed class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : class
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    private readonly ILogger<ValidationBehavior<TRequest, TResponse>> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="ValidationBehavior{TRequest, TResponse}"/>.
    /// </summary>
    /// <param name="validators">All registered validators for <typeparamref name="TRequest"/>.</param>
    /// <param name="logger">Logger instance.</param>
    public ValidationBehavior(
        IEnumerable<IValidator<TRequest>> validators,
        ILogger<ValidationBehavior<TRequest, TResponse>> logger)
    {
        _validators = validators;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next();

        _logger.LogDebug("Validating {RequestType}", typeof(TRequest).Name);

        var context = new ValidationContext<TRequest>(request);

        var failures = (await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken))))
            .SelectMany(r => r.Errors)
            .Where(e => e is not null)
            .ToList();

        if (failures.Count == 0)
            return await next();

        // Log each validation failure
        foreach (var failure in failures)
        {
            _logger.LogWarning(
                "Validation error for {RequestType}.{Property}: {Message}",
                typeof(TRequest).Name, failure.PropertyName, failure.ErrorMessage);
        }

        // Build composite error from first failure; the rest are included in the description
        var firstFailure = failures[0];
        var error = Error.Validation(firstFailure.PropertyName, firstFailure.ErrorMessage);

        // Use reflection to call the static Failure factory common to Result<T>
        var failureMethod = typeof(TResponse)
            .GetMethod("Failure", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);

        if (failureMethod is not null)
            return (TResponse)failureMethod.Invoke(null, new object[] { error })!;

        // Fallback: re-throw a domain exception so callers always get a typed error
        throw new DomainException(error);
    }
}
