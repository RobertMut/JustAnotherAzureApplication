using FluentValidation.Results;

namespace Application.Common.Exceptions
{
    /// <summary>
    /// Class ValidationException
    /// </summary>
    public class ValidationException : Exception
    {
        /// <summary>
        /// Initializes new instance of <see cref="ValidationException" /> class.
        /// </summary>
        public ValidationException()
            : base("One or more validation failures have occurred")
        {
            Errors = new Dictionary<string, string[]>();
        }

        /// <summary>
        /// Initializes new instance of <see cref="ValidationException" /> class.
        /// </summary>
        /// <param name="failures">The list of validation failures</param>
        public ValidationException(IEnumerable<ValidationFailure> failures) : this()
        {
            Errors = failures
                .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
        }

        /// <summary>
        /// Validation errors
        /// </summary>
        public IDictionary<string, string[]> Errors { get; }
    }
}
