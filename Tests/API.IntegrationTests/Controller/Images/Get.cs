using API.Controllers;
using API.IntegrationTests.Common;
using NUnit.Framework;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace API.IntegrationTests.Controller.Images
{
    public class Get
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
        public async Task GetImage()
        {
            var response = await _client.GetAsync("/api/Images/miniature-sample.jpeg");
            response.EnsureSuccessStatusCode();
            Assert.AreEqual(response.Content.Headers.ContentType, MediaTypeHeaderValue.Parse("image/jpeg"));
            byte[]? bytes = await response.Content.ReadAsByteArrayAsync();
            Assert.True(bytes.Length > 0);
        }
        [Test]
        public async Task GetUnknown()
        {
            var response = await _client.GetAsync("/api/Images/miniature-unknown.tiff");
            Assert.False(response.IsSuccessStatusCode);
        }
    }
}
