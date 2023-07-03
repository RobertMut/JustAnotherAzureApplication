using Application.Common.Exceptions;
using System.Net;

namespace Application.Common.Helpers.Exception;

public static class StatusCode
{
    /// <summary>
    /// Check if expected status code met current
    /// </summary>
    /// <param name="expected">Expected http status code</param>
    /// <param name="actual">Actual status code</param>
    /// <param name="sender">Sender</param>
    /// <exception cref="OperationFailedException">When expected status code and actual status code differs</exception>
    public static void Check(HttpStatusCode expected, HttpStatusCode actual, object sender)
    {
        if (expected != actual) throw new OperationFailedException(expected, actual, nameof(sender));
    }
}