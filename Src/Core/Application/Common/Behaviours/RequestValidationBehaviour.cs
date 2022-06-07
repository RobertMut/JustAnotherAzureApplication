using FluentValidation;
using MediatR;
using ValidationException = Application.Common.Exceptions.ValidationException;

namespace Application.Common.Behaviours
{
    /// <summary>
    /// Class RequestValidationBehaviour
    /// </summary>
    /// <typeparam name="TRequest">Request</typeparam>
    /// <typeparam name="TResponse">Expected response type</typeparam>
    public class RequestValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        /// <summary>
        /// Initializes new instance of <see cref="RequestValidationBehaviour{TRequest,TResponse}" /> class.
        /// </summary>
        /// <param name="validators">Validators list</param>
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
                var validationResults = await Task.WhenAll(
                    _validators.Select(v =>
                        v.ValidateAsync(context, cancellationToken)));

                var failures = validationResults
                    .Where(r => r.Errors.Any())
                    .SelectMany(r => r.Errors)
                    .ToList();
                if (failures.Any())
                {
                    throw new ValidationException(failures);
                }   
            }

            return await next();
        }
    }
}
