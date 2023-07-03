using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.IntegrationTests.Common;

/// <summary>
/// Class MockSchemeProvider
/// </summary>
public class MockSchemeProvider : AuthenticationSchemeProvider
{
    public MockSchemeProvider(IOptions<AuthenticationOptions> options)
        : base(options)
    {
    }
    
    protected MockSchemeProvider(IOptions<AuthenticationOptions> options,
        IDictionary<string, AuthenticationScheme> schemes) : base(options, schemes)
    {
    }

    /// <summary>
    /// Gets authentication scheme from name
    /// </summary>
    /// <param name="name">Scheme name</param>
    /// <returns><see cref="AuthenticationScheme"/></returns>
    public override Task<AuthenticationScheme> GetSchemeAsync(string name)
    {
        if (name == JwtBearerDefaults.AuthenticationScheme)
        {
            var scheme = new AuthenticationScheme(
                JwtBearerDefaults.AuthenticationScheme,
                JwtBearerDefaults.AuthenticationScheme,
                typeof(MockAuthenticationHandler)
            );
            return Task.FromResult(scheme);
        }

        return base.GetSchemeAsync(name);
    }
}