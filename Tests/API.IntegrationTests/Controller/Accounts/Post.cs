using API.Controllers;
using API.IntegrationTests.Common;
using Application.Common.Models;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace API.IntegrationTests.Controller.Accounts
{
    public class Post
    {
        private CustomWebApplicationFactory<AccountsController> _factory;
        private HttpClient _client;

        [SetUp]
        public async Task SetUp()
        {
            _factory = new CustomWebApplicationFactory<AccountsController>();
            _client = await _factory.GetAuthenticatedClient();
        }

        [Test]
        public async Task Login()
        {
            var content = new StringContent(JsonConvert.SerializeObject(new LoginModel
            {
                UserName = "Default",
                Password = "12345"
            }), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("api/Accounts/", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            Assert.True(responseContent.Contains("token"));
            response.EnsureSuccessStatusCode();
        }

        [Test]
        public async Task LoginFailedWrongCredentials()
        {
            var content = new StringContent(JsonConvert.SerializeObject(new LoginModel
            {
                UserName = "Test",
                Password = "1234"
            }), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("api/Accounts/", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            Assert.False(responseContent.Contains("token"));
            Assert.Throws<HttpRequestException>(() =>
            {
                response.EnsureSuccessStatusCode();
            });
        }
    }
}
