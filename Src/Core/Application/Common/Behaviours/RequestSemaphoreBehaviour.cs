using Application.Common.Interfaces.Blob;
using MediatR;
using System.Reflection;

namespace Application.Common.Behaviours;

/// <summary>
/// RequestSemaphoreBehaviour
/// </summary>
/// <typeparam name="TRequest">Request with filename</typeparam>
/// <typeparam name="TResponse">Expected response type</typeparam>
public class RequestSemaphoreBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly IBlobLeaseManager _blobLeaseManager;
    
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
        string id = string.Empty;
        if (requestType.FullName.ToLower().Contains("group"))
        {
            id = requestType.GetProperty("GroupId", BindingFlags.Public | BindingFlags.Instance)?
                .GetValue(request).ToString();
        }
        else
        {
            id = requestType.GetProperty("UserId", BindingFlags.Public | BindingFlags.Instance)?
                .GetValue(request).ToString();
        }

        if (string.IsNullOrEmpty(filename) && string.IsNullOrEmpty(id))
        {
            return await next();
        }
        string semaphoreName = $"{filename}{id}";

        bool semaphoreExist = Semaphore.TryOpenExisting(semaphoreName, out var semaphore);
        string leaseId = await _blobLeaseManager.SetBlobName(semaphoreName).GetLease(cancellationToken);

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