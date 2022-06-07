namespace Application.Common.Exceptions
{
    /// <summary>
    /// Class UnauthorizedException
    /// </summary>
    public class UnauthorizedException : Exception
    {
        /// <summary>
        /// Initializes new instance of <see cref="UnauthorizedException" /> class.
        /// </summary>
        /// <param name="user">The user</param>
        public UnauthorizedException(string user) : base($"Invalid credentials for user {user}")
        {
        }
    }
}
