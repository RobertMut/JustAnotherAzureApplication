using System.Net;

namespace Application.Common.Exceptions
{
    /// <summary>
    /// Class OperationFailedException
    /// </summary>
    public class OperationFailedException : Exception
    {
        /// <summary>
        /// Initializes new instance of <see cref="OperationFailedException" /> class.
        /// </summary>
        /// <param name="expectedCode">The expected http code</param>
        /// <param name="resultCode">The given http code</param>
        /// <param name="where">The class</param>
        /// <param name="inner">Inner exception</param>
        public OperationFailedException(HttpStatusCode expectedCode, HttpStatusCode resultCode, string where, string? inner = null) :
            base($"Operation failed expected {expectedCode} status code, but was {resultCode}.\n" +
                $"In {where}.\n{inner}")
        {
        }
    }
}
