using System.Net.Http.Headers;
using API.AutomatedTests.Implementation.Common.Interfaces;
using Microsoft.Extensions.Configuration;

namespace API.AutomatedTests.Infrastructure.API;

public class HttpCallerService : IHttpCallerService
{
    private readonly string baseAddress;
    private readonly HttpClient httpClient;

    public HttpCallerService(IConfiguration configuration)
    {
        string listenAddress = configuration["ListenAddress"];
        string listenPort = configuration["ListenPort"];
        if (string.IsNullOrEmpty(listenAddress) && string.IsNullOrEmpty(listenPort))
        {
            throw new Exception("Base address or port does not exists!");
        }

        baseAddress = $"{listenAddress}:{listenPort}/";
        httpClient = new HttpClient
        {
            BaseAddress = new Uri(baseAddress)
        };
    }

    public async Task<HttpResponseMessage> MakePostCall(string endpoint, string method, HttpContent content, CancellationToken ct, AuthenticationHeaderValue authenticationHeaderValue = null)
    {
        httpClient.DefaultRequestHeaders.Authorization = authenticationHeaderValue;
        var response = await httpClient.SendAsync(new HttpRequestMessage
        {
            Content = content,
            Method = new HttpMethod(method),
            RequestUri = new Uri(baseAddress+endpoint),
        }, ct);
        
        return response;
    }
    
}