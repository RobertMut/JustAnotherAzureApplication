using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using API.Service;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Presentation.UnitTests.Service;

[ExcludeFromCodeCoverage]
[TestFixture]
public class CurrentUserServiceTests
{
    [Test]
    public async Task CurrentUserServiceSetsUserIdPropertyCorrectly()
    {
        Mock<IHttpContextAccessor> httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        httpContextAccessorMock.Setup(x => x.HttpContext)
            .Returns(() =>
            {
                var contextMock = new Mock<HttpContext>();
                contextMock.Setup(x => x.User)
                    .Returns(new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new(ClaimTypes.NameIdentifier, "user")
                    })));
                return contextMock.Object;
            });
        
        CurrentUserService currentUserService = new CurrentUserService(httpContextAccessorMock.Object);
        
        Assert.IsNotEmpty(currentUserService.UserId);
        Assert.That(currentUserService.UserId, Is.EqualTo("user"));
    }
    
    [Test]
    public async Task CurrentUserServiceDoesNotSetUserId()
    {
        Mock<IHttpContextAccessor> httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        httpContextAccessorMock.Setup(x => x.HttpContext)
            .Returns(() =>
            {
                var contextMock = new Mock<HttpContext>();
                contextMock.Setup(x => x.User)
                    .Returns(new ClaimsPrincipal(new ClaimsIdentity(Array.Empty<Claim>())));
                return contextMock.Object;
            });
        
        CurrentUserService currentUserService = new CurrentUserService(httpContextAccessorMock.Object);
        
        Assert.IsNull(currentUserService.UserId);
    }
}