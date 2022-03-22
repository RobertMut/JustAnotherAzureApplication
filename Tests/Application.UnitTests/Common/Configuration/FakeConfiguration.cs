namespace Application.UnitTests.Common.Configuration
{
    public class FakeConfiguration
    {
        public const string Configuration = "{\n \"JWT\": {\n" +
        "\"ValidIssuer\": \"https://localhost/\",\n" +
        "\"ValidAudience\": \"https://localhost\"\n" +
        "},\n" +
        "\"JWTSecret\": \"Secret\"\n}";
    }
}
