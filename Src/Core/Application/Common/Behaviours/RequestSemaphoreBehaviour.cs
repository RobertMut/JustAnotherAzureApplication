using Application.Common.Interfaces.Blob;
using MediatR;
using System.Reflection;

namespace Application.Common.Behaviours
{
    /// <summary>
    /// Class RequestSemaphoreBehaviour
    /// </summary>
    /// <typeparam name="TRequest">Request with filename</typeparam>
    /// <typeparam name="TResponse">Expected response type</typeparam>
    public class RequestSemaphoreBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly IBlobLeaseManager _blobLeaseManager;

        /// <summary>
        /// Initializes new instance of <see cref="RequestSemaphoreBehaviour{TRequest,TResponse}" /> class.
        /// </summary>
        /// <param name="blobLeaseManager">The lease manager</param>
        public RequestSemaphoreBehaviour(IBlobLeaseManager blobLeaseManager)
        {
            _blobLeaseManager = blobLeaseManager;
        }

        /// <summary>
        /// Requests semaphore to file and locks it
        /// </summary>
        /// <param name="request">Request with filename</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <param name="next">Request handler</param>
        /// <returns>Response</returns>
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var requestType = request.GetType();
            string filename = requestType.GetProperty("Filename", BindingFlags.Public | BindingFlags.Instance)?.GetValue(request).ToString();
            string userId = requestType.GetProperty("UserId", BindingFlags.Public | BindingFlags.Instance)?
                    .GetValue(request).ToString();

            if (string.IsNullOrEmpty(filename) && string.IsNullOrEmpty(userId))
            {
                return await next();
            }
            string semaphoreName = $"{filename}{userId}";
            _blobLeaseManager.SetBlobName(semaphoreName);

            bool semaphoreExist = Semaphore.TryOpenExisting(semaphoreName, out var semaphore);
            string leaseId = await _blobLeaseManager.GetLease(cancellationToken);

            if (!semaphoreExist)
            {
                semaphore = new Semaphore(1, 1, semaphoreName);
            }

            try
            {
                semaphore.WaitOne();

                return await next();
            } 
            finally
            {
                if (!string.IsNullOrEmpty(leaseId))
                {
                    _blobLeaseManager.ReleaseLease(leaseId, cancellationToken);
                }

                semaphore.Release();
            }
        }
    }
}
