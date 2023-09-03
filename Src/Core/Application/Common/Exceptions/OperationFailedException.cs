using System.Net;

namespace Application.Common.Exceptions;

public class OperationFailedException : Exception
{
    public OperationFailedException(HttpStatusCode expectedCode, HttpStatusCode resultCode, string where, string? inner = null) :
        base($"Operation failed expected {expectedCode} status code, but was {resultCode}.\n" +
             $"In {where}.\n{inner}")
    {
    }
}