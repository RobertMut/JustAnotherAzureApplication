namespace Application.Common.Exceptions
{
    internal class UnauthorizedException : Exception
    {
        public UnauthorizedException(string user) : base($"Invalid credentials for user {user}")
        {
        }
    }
}
