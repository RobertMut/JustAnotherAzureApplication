using Application.Common.Exceptions;
using System.Net;

namespace Application.Common.Helpers.Exception
{
    public static class StatusCode
    {
        public static void Check(HttpStatusCode expected, HttpStatusCode actual, object sender)
        {
            if (expected != actual) throw new OperationFailedException(expected, actual, nameof(sender));
        }
    }
}
