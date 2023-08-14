using Application.Account.Commands.Login;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Database;
using Application.Common.Interfaces.Identity;
using Application.Common.Models.Account;
using Domain.Entities;
using MediatR;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Virtuals;
using Application.UnitTests.Common.Mocking;

namespace Application.UnitTests.Accounts.Commands.Login;

[TestFixture]
[ExcludeFromCodeCoverage]
public class LoginCommandTests
{
    private IUnitOfWork _unitOfWork;
    private Mock<ITokenGenerator> _tokenGenerator;
    private LoginCommand.LoginCommandHandler _handler;
    private Mock<Repository<User>> _userRepositoryMock;

    [SetUp]
    public async Task SetUp()
    {
        _userRepositoryMock = new Mock<Repository<User>>(Mock.Of<IJAAADbContext>());
        _unitOfWork = new UnitOfWorkMock(userRepository: _userRepositoryMock)
            .GetMockedUnitOfWork();
        
        _tokenGenerator = new Mock<ITokenGenerator>();
        _tokenGenerator.Setup(x => x.GetToken(It.IsAny<User>())).ReturnsAsync(new JwtSecurityToken());
    }


    [Test]
    public async Task LoginTest()
    {
        var jwt = new JwtSecurityToken();
        _tokenGenerator.Setup(x => x.GetToken(It.IsAny<User>()))
            .ReturnsAsync(jwt);
        _userRepositoryMock.Setup(x =>
                x.GetObjectBy(It.IsAny<Expression<Func<User?, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User
            {
                Id = Guid.NewGuid(),
                Username = "Default",
                Password = "123456",
            });
    
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
            
            Assert.AreEqual(responseFromHandler, jwt);
        });
    }

    [Test]
    public async Task LoginThrowsIncorrectCredentials()
    {
        _userRepositoryMock.Setup(x =>
                x.GetObjectBy(It.IsAny<Expression<Func<User?, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User
            {
                Id = Guid.NewGuid(),
                Username = "Wrong",
                Password = "Wrong2",
            });
        
        _handler = new LoginCommand.LoginCommandHandler(_unitOfWork, _tokenGenerator.Object);
        var loginQuery = new LoginCommand
        {
            LoginModel = new LoginModel
            {
                UserName = "Default",
                Password = "123456"
            }
        };
        Assert.ThrowsAsync<UnauthorizedException>(async () => await _handler.Handle(loginQuery, CancellationToken.None), $"Invalid credentials for user Default");
    }

    [Test]
    public async Task LoginThrowsUserNotFound()
    {
        _userRepositoryMock.Setup(x =>
                x.GetObjectBy(It.IsAny<Expression<Func<User?, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(default(User));
        
        _handler = new LoginCommand.LoginCommandHandler(_unitOfWork, _tokenGenerator.Object);
        var loginQuery = new LoginCommand
        {
            LoginModel = new LoginModel
            {
                UserName = "Default",
                Password = "123456"
            }
        };
        
        Assert.ThrowsAsync<UserNotFoundException>(async () => await _handler.Handle(loginQuery, CancellationToken.None), "User Default not found!");
    }
}