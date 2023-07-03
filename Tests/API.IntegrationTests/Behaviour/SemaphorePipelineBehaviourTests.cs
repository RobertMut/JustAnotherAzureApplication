using API.Controllers;
using API.IntegrationTests.Common;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace API.IntegrationTests.Behaviour;

/// <summary>
/// Class SemaphorePipelineBehaviourTests
/// </summary>
public class SemaphorePipelineBehaviourTests
{
    private CustomWebApplicationFactory<ImagesController> _factory;
    private HttpClient _client;

    /// <summary>
    /// Setup
    /// </summary>
    [SetUp]
    public async Task SetUp()
    {
        _factory = new CustomWebApplicationFactory<ImagesController>();
        _client = await _factory.GetAuthenticatedClient();
    }

    /// <summary>
    /// Does not throw on multiple calls to same file at the same time
    /// </summary>
    [Test]
    public async Task ParallelGetMutexBehaviourDoesNotThrow()
    {
        var firstGet = _client.GetAsync("/api/Images/miniature_300x300_sample1.jpeg");
        var secondGet = _client.GetAsync("/api/Images/miniature_300x300_sample1.jpeg");
        var thirdGet = _client.GetAsync("/api/Images/miniature_300x300_sample1.jpeg");

        var responses = await Task.WhenAll(firstGet, secondGet, thirdGet);

        responses[0].EnsureSuccessStatusCode();
        responses[1].EnsureSuccessStatusCode();
        responses[2].EnsureSuccessStatusCode();
    }
}