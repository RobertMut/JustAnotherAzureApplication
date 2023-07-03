using Application.Account.Commands.Login;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Database;
using Application.Common.Interfaces.Identity;
using Application.Common.Models.Account;
using Application.UnitTests.Common.Fakes;
using Domain.Entities;
using MediatR;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Application.UnitTests.Accounts.Commands.Login;

[TestFixture]
[ExcludeFromCodeCoverage]
public class LoginCommandTests
{
    private Mock<IMediator> _mediator;
    private IUnitOfWork _unitOfWork;
    private Mock<ITokenGenerator> _tokenGenerator;
    private LoginCommand.LoginCommandHandler _handler;

    [SetUp]
    public async Task SetUp()
    {
        _mediator = new Mock<IMediator>();
        _unitOfWork = new FakeUnitOfWork();
        _tokenGenerator = new Mock<ITokenGenerator>();


        _tokenGenerator.Setup(x => x.GetToken(It.IsAny<User>())).ReturnsAsync(new JwtSecurityToken());
        var claims = new List<Claim> {
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        _mediator.Setup(x => x.Send(It.IsAny<LoginCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(new JwtSecurityToken(claims: claims));
    }


    [Test]
    public async Task LoginTest()
    {
        _handler = new LoginCommand.LoginCommandHandler(_unitOfWork, _tokenGenerator.Object);
        var loginQuery = new LoginCommand
        {
            LoginModel = new LoginModel
            {
                UserName = "Default",
                Password = "123456"
            }
        };
        Assert.DoesNotThrowAsync(async () =>
        {
            var responseFromHandler = await _handler.Handle(loginQuery, CancellationToken.None);
            var response = await _mediator.Object.Send(loginQuery, CancellationToken.None);

            Assert.IsInstanceOf<JwtSecurityToken>(responseFromHandler);
            Assert.IsInstanceOf<JwtSecurityToken>(response);
        });
    }

    [Test]
    public async Task LoginThrowsUserNotFound()
    {
        _handler = new LoginCommand.LoginCommandHandler(_unitOfWork, _tokenGenerator.Object);


        var loginQuery = new LoginCommand
        {
            LoginModel = new LoginModel
            {
                UserName = "User",
                Password = "12345"
            }
        };

        Assert.ThrowsAsync<UserNotFoundException>(async () =>
        {
            var responseFromHandler = await _handler.Handle(loginQuery, CancellationToken.None);
        });
    }

    [Test]
    public async Task LoginThrowsUnauthorized()
    {
        _handler = new LoginCommand.LoginCommandHandler(_unitOfWork, _tokenGenerator.Object);


        var loginQuery = new LoginCommand
        {
            LoginModel = new LoginModel
            {
                UserName = "Default",
                Password = "1234"
            }
        };

        Assert.ThrowsAsync<UnauthorizedException>(async () =>
        {
            var responseFromHandler = await _handler.Handle(loginQuery, CancellationToken.None);
        });
    }
}