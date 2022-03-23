using Application.Account.Commands.LoginCommand;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Database;
using Application.Common.Interfaces.Identity;
using Application.Common.Models;
using Domain.Entities;
using Infrastructure.Persistence;
using MediatR;
using Moq;
using NUnit.Framework;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading;
using System.Threading.Tasks;

namespace Application.UnitTests.Accounts.Commands.Login
{
    [TestFixture]
    public class LoginCommandTests
    {
        private Mock<IMediator> _mediator;
        private Mock<IRepository<User>> _userRepository;
        private Mock<ITokenGenerator> _tokenGenerator;
        private JAAADbContext _jaaaDbContext;
        private LoginCommand.LoginCommandHandler _handler;

        [SetUp]
        public async Task SetUp()
        {
            _mediator = new Mock<IMediator>();
            _userRepository = new Mock<IRepository<User>>();
            _tokenGenerator = new Mock<ITokenGenerator>();

            _tokenGenerator.Setup(x => x.GetToken(It.IsAny<User>())).ReturnsAsync(new JwtSecurityToken());
            _mediator.Setup(x => x.Send(It.IsAny<LoginCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(new JwtSecurityToken());
        }


        [Test]
        public async Task LoginTest()
        {
            _userRepository.Setup(x => x.GetByNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(new User
            {
                Username = "Default",
                Password = "123456",
                Id = Guid.NewGuid()
            });

            _handler = new LoginCommand.LoginCommandHandler(_userRepository.Object, _tokenGenerator.Object);
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
        public async Task LoginThrowsUnauthorized()
        {
            _userRepository.Setup(x => x.GetByNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(new User());
            _handler = new LoginCommand.LoginCommandHandler(_userRepository.Object, _tokenGenerator.Object);


            var loginQuery = new LoginCommand
            {
                LoginModel = new LoginModel
                {
                    UserName = "User",
                    Password = "12345"
                }
            };

            Assert.ThrowsAsync<UnauthorizedException>(async () =>
            {
                var responseFromHandler = await _handler.Handle(loginQuery, CancellationToken.None);
            });
        }
    }
}
