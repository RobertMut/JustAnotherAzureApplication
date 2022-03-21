using Application.Common.Interfaces.Blob;
using MediatR;
using System.Reflection;

namespace Application.Common.Behaviours
{
    public class RequestSemaphoreBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly IBlobLeaseManager _blobLeaseManager;

        public RequestSemaphoreBehaviour(IBlobLeaseManager blobLeaseManager)
        {
            _blobLeaseManager = blobLeaseManager;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var requestType = request.GetType();
            string semaphoreName = requestType.GetProperty("Filename", BindingFlags.Public | BindingFlags.Instance)?
                .GetValue(request).ToString() 
                ?? requestType.GetProperty("Guid", BindingFlags.Public | BindingFlags.Instance)?
                .GetValue(request).ToString();

            if (string.IsNullOrEmpty(semaphoreName))
            {
                return await next();
            }
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
