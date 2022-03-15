using MediatR;

namespace Application.Common.Behaviours
{
    public class RequestMutexBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            string filename = request.GetType()
                .GetProperty("Filename", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                .GetValue(request).ToString();
            bool mutexExist = Mutex.TryOpenExisting(filename, out var mutex);

            if (!mutexExist)
            {
                mutex = new Mutex(false, filename);
            }

            try
            {
                mutex.WaitOne();

                return await next();
            } finally
            {
                mutex.ReleaseMutex();
            }
        }
    }
}
