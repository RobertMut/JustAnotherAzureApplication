using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.IntegrationTests.Common
{
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

        public override Task<AuthenticationScheme> GetSchemeAsync(string name)
        {
            if (name == JwtBearerDefaults.AuthenticationScheme)
            {
                var scheme = new AuthenticationScheme(
                    "Test",
                    "Test",
                    typeof(MockAuthenticationHandler)
                );
                return Task.FromResult(scheme);
            }

            return base.GetSchemeAsync(name);
        }
    }
}
