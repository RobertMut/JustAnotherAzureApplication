using System.Diagnostics.CodeAnalysis;
using System.Text;
using API.Controllers;
using Application.Account.Commands.Login;
using Application.Account.Commands.Register;
using Application.Common.Models.Account;
using Domain.Entities;
using Infrastructure.Services.Identity;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Presentation.UnitTests.Controller;

[ExcludeFromCodeCoverage]
[TestFixture]
public class AccountsControllerTests
{
    private Mock<IMediator> _mediatorMock;
    private IConfigurationRoot _configuration;

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
        _mediatorMock = new Mock<IMediator>();
        var configurationBuilder = new ConfigurationBuilder();
        configurationBuilder.AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(SampleAppsettings)));
        _configuration = configurationBuilder.Build();
    }

    [Test]
    public async Task LoginTest()
    {
        AccountsController accountsController = new AccountsController(_mediatorMock.Object);
        _mediatorMock.Setup(x => x.Send(It.IsAny<LoginCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(await new TokenGenerator(_configuration).GetToken(new User()));
        
        Assert.DoesNotThrowAsync(async () =>
        {
            var response = await accountsController.Login(new LoginModel()
            {
                UserName = "user",
                Password = "pass"
            });
        
        
            Assert.IsAssignableFrom(typeof(OkObjectResult), response);
        });
    }
    
    [Test]
    public async Task RegisterTest()
    {
        AccountsController accountsController = new AccountsController(_mediatorMock.Object);
        _mediatorMock.Setup(x => x.Send(It.IsAny<RegisterCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Unit.Value);
        
        Assert.DoesNotThrowAsync(async () =>
        {
            var response = await accountsController.Register(new RegisterModel()
            {
                Username = "user",
                Password = "pass"
            });
        
        
            Assert.IsAssignableFrom(typeof(OkResult), response);
        });
    }
}