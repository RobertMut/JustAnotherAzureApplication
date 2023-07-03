using API.Controllers;
using API.IntegrationTests.Common;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace API.IntegrationTests.Controller.Images;

public class Post
{
    private HttpClient _client;
    private byte[] _sample;

    [SetUp]
    public async Task SetUp()
    {
        _sample = new byte[] { 00, 00, 00, 00, 11, 22, 33 };
        _client = GlobalSetupFixture.AuthenticatedHttpClient;
    }

    [Test]
    public async Task PostNewImage()
    {
        var content = Utils.CreateSampleFile(_sample, "image/jpeg", "test1.jpeg");
        content.Add(new StringContent("100"), "width");
        content.Add(new StringContent("100"), "height");
        content.Add(new StringContent("tiff"), "targetType");

        var response = await _client.PostAsync("api/Images/", content);

        response.EnsureSuccessStatusCode();
    }

    [Test]
    public async Task ThrowValidationFailed()
    {
        var content = Utils.CreateSampleFile(_sample, "image/png", "test2.png");
        content.Add(new StringContent("0"), "width");
        content.Add(new StringContent("0"), "height");
        content.Add(new StringContent("jpeg"), "targetType");

        var response = await _client.PostAsync("api/Images/", content);

        Assert.True(response.StatusCode == System.Net.HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task WrongFormDataCausesBadRequest()
    {
        var content = new MultipartFormDataContent();
        content.Add(new StringContent("100"), "width");
        content.Add(new StringContent("100"), "height");
        content.Add(new StringContent("test3"), "file", "file");

        var response = await _client.PostAsync("api/Images/", content);

        Assert.True(response.StatusCode == System.Net.HttpStatusCode.BadRequest);
    }
}