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
using Application.Account.Commands.Register;
using Application.Common.Virtuals;
using Application.UnitTests.Common.Mocking;

namespace Application.UnitTests.Accounts.Commands.Register;

[TestFixture]
[ExcludeFromCodeCoverage]
public class RegisterCommandTests
{
    private IUnitOfWork _unitOfWork;
    private Mock<ITokenGenerator> _tokenGenerator;
    private RegisterCommand.RegisterCommandHandler _handler;
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
    public async Task RegisterTest()
    {
        _userRepositoryMock.Setup(x =>
                x.GetObjectBy(It.IsAny<Expression<Func<User?, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(default(User));
        _userRepositoryMock.Setup(x =>
            x.InsertAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()));
        
        _handler = new RegisterCommand.RegisterCommandHandler(_unitOfWork);
        var registerQuery = new RegisterCommand()
        {
            RegisterModel = new RegisterModel()
            {
                Username = "Default",
                Password = "123456"
            }
        };
        
        Assert.DoesNotThrowAsync(async () => await _handler.Handle(registerQuery, CancellationToken.None));
        
        _userRepositoryMock.Verify(x =>
            x.InsertAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task RegisterThrowsDuplicatedException()
    {
        _userRepositoryMock.Setup(x =>
                x.GetObjectBy(It.IsAny<Expression<Func<User?, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User
            {
                Id = Guid.NewGuid(),
                Username = "Default",
                Password = "123456",
            });

        
        _handler = new RegisterCommand.RegisterCommandHandler(_unitOfWork);
        var registerQuery = new RegisterCommand()
        {
            RegisterModel = new RegisterModel()
            {
                Username = "Default",
                Password = "123456"
            }
        };
        
        Assert.ThrowsAsync<DuplicatedException>(async () => await _handler.Handle(registerQuery, CancellationToken.None), $"Default already exist!");
        
        _userRepositoryMock.Verify(x =>
            x.InsertAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}