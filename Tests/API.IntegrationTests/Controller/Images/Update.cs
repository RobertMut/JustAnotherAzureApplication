using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;

namespace API.IntegrationTests.Controller.Images
{
    [TestFixture]
    public class Update
    {
        private HttpClient _client;

        [SetUp]
        public async Task SetUp()
        {
            _client = GlobalSetupFixture.AuthenticatedHttpClient;
        }

        [Test]
        public async Task UpdateImage()
        {
            var content = GenerateFormData("sample1.png", 50, 50, "tiff", 1);

            var fromPut = await _client.PutAsync("api/Images/", content);
            fromPut.EnsureSuccessStatusCode();

            var fromGet = await _client.GetAsync("/api/Images/miniature_50x50_sample1.tiff");
            fromGet.EnsureSuccessStatusCode();

            var bytes = await fromGet.Content.ReadAsByteArrayAsync();
            Assert.NotNull(bytes);
        }

        [Test]
        public async Task ThrowInternalServerErrorStatusUnknownFile()
        {
            var content = GenerateFormData("unknown.bmp", 50, 50, "tiff", 1);
            var fromPut = await _client.PutAsync("api/Images/", content);

            Assert.True(fromPut.StatusCode == System.Net.HttpStatusCode.InternalServerError);
        }

        [Test]
        public async Task ThrowValidationFailed()
        {
            var content = GenerateFormData("sample1.png", 0, 0, "");
            var fromPut = await _client.PutAsync("api/Images/", content);

            Assert.True(fromPut.StatusCode == System.Net.HttpStatusCode.BadRequest);
        }

        private MultipartFormDataContent GenerateFormData(string filename, int width, int height, string contentType, int? version = 0)
        {
            var content = new MultipartFormDataContent();
            content.Add(new StringContent(filename), "file");
            content.Add(new StringContent(width.ToString()), "width");
            content.Add(new StringContent(height.ToString()), "height");
            content.Add(new StringContent(contentType), "targetType");

            if (version != null)
            {
                content.Add(new StringContent(version.Value.ToString()), "version");
            }
                
            return content;
        }
    }
}
