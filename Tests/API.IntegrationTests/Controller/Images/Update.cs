using API.Controllers;
using API.IntegrationTests.Common;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace API.IntegrationTests.Controller.Images
{
    public class Update
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
        public async Task UpdateImage()
        {
            var content = new MultipartFormDataContent();
            content.Add(new StringContent("sample.png"), "file");
            content.Add(new StringContent("50"), "width");
            content.Add(new StringContent("50"), "height");
            content.Add(new StringContent("tiff"), "targetType");
            content.Add(new StringContent("1"), "version");
            var fromPut = await _client.PutAsync("api/Images/", content);
            var fromGet = await _client.GetAsync("/api/Images/miniature-sample.tiff");
            fromPut.EnsureSuccessStatusCode();
            var bytes = await fromGet.Content.ReadAsByteArrayAsync();
            Assert.AreEqual(bytes[0], 10);
        }

    }
}
