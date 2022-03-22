using Application.Account.Commands.LoginCommand;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Identity;
using Application.Common.Models;
using Application.UnitTests.Common.Configuration;
using Domain.Entities;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JAAADbContextFactory = Application.UnitTests.Common.Mocks.JAAADbContextFactory;

namespace Application.UnitTests.Accounts.Commands.Login
{
    [TestFixture]
    public class LoginCommandTests
    {
        private Mock<IMediator> _mediator;
        private Mock<IUserManager> _userManager;
        private IConfigurationRoot _configuration;
        private JAAADbContext _jaaaDbContext;
        private LoginCommand.LoginCommandHandler _handler;

        [SetUp]
        public async Task SetUp()
        {
            _jaaaDbContext = JAAADbContextFactory.Create();
            _mediator = new Mock<IMediator>();
            _userManager = new Mock<IUserManager>();

            _configuration = new ConfigurationBuilder()
                .AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(FakeConfiguration.Configuration)))
                .Build();
            _userManager.Setup(x => x.GetUserByNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(new User
            {
                Username = "Default",
                Password = "123456",
                Id = JAAADbContextFactory.ProfileId
            });

            _handler = new LoginCommand.LoginCommandHandler(_userManager.Object, _configuration);

            _mediator.Setup(x => x.Send(It.IsAny<LoginCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(new JwtSecurityToken());
        }

        [TearDown]
        public async Task TearDown()
        {
            JAAADbContextFactory.Destroy(_jaaaDbContext);
        }

        [Test]
        public async Task LoginTest()
        {
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
