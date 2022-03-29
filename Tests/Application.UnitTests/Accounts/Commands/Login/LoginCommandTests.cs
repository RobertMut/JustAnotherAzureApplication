using Application.Account.Commands.Login;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Database;
using Application.Common.Interfaces.Identity;
using Application.Common.Models.Account;
using Domain.Entities;
using Infrastructure.Persistence;
using MediatR;
using Moq;
using NUnit.Framework;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Application.UnitTests.Accounts.Commands.Login
{
    [TestFixture]
    public class LoginCommandTests
    {
        private Mock<IMediator> _mediator;
        private Mock<IUnitOfWork> _unitOfWork;
        private Mock<ITokenGenerator> _tokenGenerator;
        private JAAADbContext _jaaaDbContext;
        private LoginCommand.LoginCommandHandler _handler;

        [SetUp]
        public async Task SetUp()
        {
            _mediator = new Mock<IMediator>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _tokenGenerator = new Mock<ITokenGenerator>();


            _tokenGenerator.Setup(x => x.GetToken(It.IsAny<User>())).ReturnsAsync(new JwtSecurityToken());
            _mediator.Setup(x => x.Send(It.IsAny<LoginCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(new JwtSecurityToken());
        }


        [Test]
        public async Task LoginTest()
        {
            _unitOfWork.Setup(x => x.UserRepository.GetObjectBy(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>())).ReturnsAsync(new User
            {
                Username = "Default",
                Password = "123456",
                Id = Guid.NewGuid()
            });

            _handler = new LoginCommand.LoginCommandHandler(_unitOfWork.Object, _tokenGenerator.Object);
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
            _unitOfWork.Setup(x => x.UserRepository.GetObjectBy(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>())).ReturnsAsync(new User());
            _handler = new LoginCommand.LoginCommandHandler(_unitOfWork.Object, _tokenGenerator.Object);


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
