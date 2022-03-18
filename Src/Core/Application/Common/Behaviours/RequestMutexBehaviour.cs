using Application.Common.Interfaces.Blob;
using MediatR;
using System.Reflection;

namespace Application.Common.Behaviours
{
    public class RequestMutexBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly IBlobLeaseManager _blobLeaseManager;

        public RequestMutexBehaviour(IBlobLeaseManager blobLeaseManager)
        {
            _blobLeaseManager = blobLeaseManager;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var requestType = request.GetType();
            string mutexName = requestType.GetProperty("Filename", BindingFlags.Public | BindingFlags.Instance)?
                .GetValue(request).ToString() 
                ?? requestType.GetProperty("Guid", BindingFlags.Public | BindingFlags.Instance)?
                .GetValue(request).ToString();

            if (string.IsNullOrEmpty(mutexName))
            {
                return await next();
            }
            _blobLeaseManager.SetBlobName(mutexName);

            bool mutexExist = Mutex.TryOpenExisting(mutexName, out var mutex);
            string leaseId = await _blobLeaseManager.GetLease(cancellationToken);

            if (!mutexExist)
            {
                mutex = new Mutex(false, mutexName);
            }

            try
            {
                mutex.WaitOne();

                return await next();
            } 
            finally
            {
                if (!string.IsNullOrEmpty(leaseId))
                {
                    _blobLeaseManager.ReleaseLease(leaseId, cancellationToken);
                }

                mutex.ReleaseMutex();
            }
        }
    }
}
