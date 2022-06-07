﻿using API.Controllers;
using API.IntegrationTests.Common;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace API.IntegrationTests.Controller.Images
{
    [TestFixture]
    public class Delete
    {
        private HttpClient _client;

        [SetUp]
        public async Task SetUp()
        {
            _client = GlobalSetupFixture.AuthenticatedHttpClient;
        }

        [Test]
        public async Task DeleteImageWithMiniatures()
        {
            var response = await _client.DeleteAsync("/api/Images/sample3.png");
            response.EnsureSuccessStatusCode();
            var responseGetFirstMiniature = await _client.GetAsync("/api/Images/miniature_300x300_sample3.jpeg");

            Assert.Throws<HttpRequestException>(() => 
            {
                responseGetFirstMiniature.EnsureSuccessStatusCode();
            });
        }

        [Test]
        public async Task DeleteMiniaturesOnly()
        {
            var response = await _client.DeleteAsync("/api/Images/sample2.png/True");
            response.EnsureSuccessStatusCode();
            var responseGetMiniature = await _client.GetAsync("/api/Images/miniature_400x400_sample2.jpeg");
            var responseGetOtherOriginal = await _client.GetAsync("/api/Images/original_sample2.png");

            Assert.Throws<HttpRequestException>(() => responseGetMiniature.EnsureSuccessStatusCode());
            responseGetOtherOriginal.EnsureSuccessStatusCode();
        }

        [Test]
        public async Task DeleteMiniatureWithSize()
        {
            var response = await _client.DeleteAsync("/api/Images/sample1.png/True/200x200");
            response.EnsureSuccessStatusCode();
            var responseGetFirstMiniature = await _client.GetAsync("/api/Images/miniature_300x300_sample1.jpeg");
            var responseGetDeletedMiniature = await _client.GetAsync("/api/Images/miniature_200x200_sample1.jpeg");
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
