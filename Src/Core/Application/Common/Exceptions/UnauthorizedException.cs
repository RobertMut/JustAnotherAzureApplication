namespace Application.Common.Exceptions
{
    public class UnauthorizedException : Exception
    {
        public UnauthorizedException(string user) : base($"Invalid credentials for user {user}")
        {
        }
    }
}
