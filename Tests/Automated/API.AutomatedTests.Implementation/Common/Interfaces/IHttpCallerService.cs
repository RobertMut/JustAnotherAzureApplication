using System.Net.Http.Headers;

namespace API.AutomatedTests.Implementation.Common.Interfaces;

public interface IHttpCallerService
{
    Task<HttpResponseMessage> MakePostCall(string endpoint, string method, HttpContent content, CancellationToken ct,
        AuthenticationHeaderValue authenticationHeaderValue = null);
}