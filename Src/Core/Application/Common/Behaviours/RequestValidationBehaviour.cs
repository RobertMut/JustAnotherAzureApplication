using FluentValidation;
using MediatR;
using ValidationException = Application.Common.Exceptions.ValidationException;

namespace Application.Common.Behaviours;

/// <summary>
/// RequestValidationBehaviour
/// </summary>
/// <typeparam name="TRequest">Request</typeparam>
/// <typeparam name="TResponse">Expected response type</typeparam>
public class RequestValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public RequestValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    /// <summary>
    /// Validation Handler
    /// </summary>
    /// <param name="request">Request</param>
    /// <param name="cancellationToken">CancellationToken</param>
    /// <param name="next">Request handler</param>
    /// <returns>Response</returns>
    /// <exception cref="ValidationException">When validation expectations were unmet</exception>
    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);

            var failures = _validators
                .Select(v => v.Validate(context))
                .SelectMany(result => result.Errors)
                .Where(f => f != null)
                .ToList();
            
            if (failures.Count != 0)
            {
                throw new ValidationException(failures);
            }   
        }

        return await next();
    }
}