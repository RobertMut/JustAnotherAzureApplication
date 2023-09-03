using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;
using Domain.Entities;
using Infrastructure.Services.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.UnitTests.Services.Identity;

[ExcludeFromCodeCoverage]
[TestFixture]
public class TokenGeneratorTests
{
    private IConfiguration _configuration;
    private ConfigurationBuilder _configurationBuilder;

    private const string SampleAppsettings = @"{
        ""JWT"": {
        ""ValidIssuer"": ""issuer"",
        ""ValidAudience"": ""audience"",
        ""AllowDangerousCertificate"": ""True""},
        ""JWTSecret"": ""secret""
        }";

    [SetUp]
    public async Task SetUp()
    {
        _configurationBuilder = new ConfigurationBuilder();
    }

    [Test]
    public async Task GetTokenGeneratesToken()
    {
        var userId = Guid.NewGuid();
        _configurationBuilder.AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(SampleAppsettings)));
        _configuration = _configurationBuilder.Build();

        var tokenGenerator = new TokenGenerator(_configuration);

        var token = await tokenGenerator.GetToken(new User
        {
            Id = userId,
            Username = "user",
            Password = "pass",
        });

        var secret = MD5.HashData(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("JWTSecret")));
        var authSigningKey = new SymmetricSecurityKey(secret);

        Assert.True(token.Issuer == "issuer");
        Assert.True(token.Audiences.ToArray()[0] == "audience");
        Assert.True(token.Claims.Any(x => x.Value == userId.ToString()));
        Assert.True(token.SigningCredentials.Key.KeySize == authSigningKey.KeySize);
    }
}