using System.Runtime.Serialization;
using System.Text.Json;
using FluentValidation.Results;

namespace Application.Common.Exceptions;

public class ValidationException : Exception
{
    public ValidationException()
        : base("One or more validation failures have occurred")
    {
        Errors = new Dictionary<string, string[]>();
    }
    
    public ValidationException(IEnumerable<ValidationFailure> failures) : base(GetErrorsToString(failures))
    {
        Errors = GetErrors(failures);
    }

    /// <summary>
    /// Validation errors
    /// </summary>
    public IDictionary<string, string[]> Errors { get; }

    private static string GetErrorsToString(IEnumerable<ValidationFailure> failures)
    {
        return JsonSerializer.Serialize(GetErrors(failures));
    }

    private static Dictionary<string, string[]> GetErrors(IEnumerable<ValidationFailure> failures) =>
        failures
            .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
}