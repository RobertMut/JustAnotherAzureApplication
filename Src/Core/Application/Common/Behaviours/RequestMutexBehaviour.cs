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
            string filename = request.GetType()
                .GetProperty("Filename", BindingFlags.Public | BindingFlags.Instance)?
                .GetValue(request).ToString() ?? "NoFileMutex";
            _blobLeaseManager.SetBlobName(filename);

            bool mutexExist = Mutex.TryOpenExisting(filename, out var mutex);
            string leaseId = await _blobLeaseManager.GetLease(cancellationToken);

            if (!mutexExist)
            {
                mutex = new Mutex(false, filename);
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
