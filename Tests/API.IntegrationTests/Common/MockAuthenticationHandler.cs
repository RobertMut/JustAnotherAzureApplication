using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace API.IntegrationTests.Common;

public class MockAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public MockAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock) : base(options, logger, encoder, clock)
    {
    }

    /// <summary>
    /// Handles Authentication
    /// </summary>
    /// <returns>Authentication Result</returns>
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (Request.Headers.ContainsKey(HeaderNames.Authorization))
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, "TestUser"),
                new(ClaimTypes.NameIdentifier, Utils.DefaultId.ToString())
            };
            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "Test");

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }

        return Task.FromResult(AuthenticateResult.Fail("Unauthorized"));
    }
}