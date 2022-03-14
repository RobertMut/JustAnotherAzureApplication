using API.Controllers;
using API.IntegrationTests.Common;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace API.IntegrationTests.Controller.Images
{
    public class Delete
    {
        private CustomWebApplicationFactory<ImagesController> _factory;
        private HttpClient _client;

        [SetUp]
        public async Task SetUp()
        {
            _factory = new CustomWebApplicationFactory<ImagesController>();
            _client = _factory.CreateClient(_factory.ClientOptions);
        }


        [Test]
        public async Task DeleteImageWithMiniatures()
        {
            var response = await _client.DeleteAsync("/api/Images/sample1.png");
            response.EnsureSuccessStatusCode();
            var responseGetFirstMiniature = await _client.GetAsync("/api/Images/miniature-300x300-sample1.jpeg");
            var responseGetSecondMiniature = await _client.GetAsync("/api/Images/miniature-200x200-sample1.jpeg");
            Assert.Throws<HttpRequestException>(() => 
            {
                responseGetFirstMiniature.EnsureSuccessStatusCode();
                responseGetSecondMiniature.EnsureSuccessStatusCode();
            });
        }

        [Test]
        public async Task DeleteMiniaturesOnly()
        {
            var response = await _client.DeleteAsync("/api/Images/sample2.png");
            response.EnsureSuccessStatusCode();
            var responseGetMiniature = await _client.GetAsync("/api/Images/miniature-400x400-sample2.jpeg");
            var responseGetOtherOriginal = await _client.GetAsync("/api/Images/original-sample1.png");
            Assert.Throws<HttpRequestException>(() => responseGetMiniature.EnsureSuccessStatusCode());
            responseGetOtherOriginal.EnsureSuccessStatusCode();
        }

        [Test]
        public async Task DeleteMiniatureWithSize()
        {
            var response = await _client.DeleteAsync("/api/Images/sample1.png/True/200x200");
            response.EnsureSuccessStatusCode();
            var responseGetFirstMiniature = await _client.GetAsync("/api/Images/miniature-300x300-sample1.jpeg");
            var responseGetDeletedMiniature = await _client.GetAsync("/api/Images/miniature-200x200-sample1.jpeg");
            responseGetFirstMiniature.EnsureSuccessStatusCode();
            Assert.Throws<HttpRequestException>(() =>
            {
                responseGetDeletedMiniature.EnsureSuccessStatusCode();
            });
        }

        [Test]
        public async Task DeleteWithNullFilename()
        {
            var response = await _client.DeleteAsync("/api/Images/");
            Assert.Throws<HttpRequestException>(() =>
            {
                response.EnsureSuccessStatusCode();
            });
        }
    }
}
