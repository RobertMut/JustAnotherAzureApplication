namespace Application.Common.Exceptions
{
    /// <summary>
    /// Class UserNotFoundException
    /// </summary>
    public class UserNotFoundException : Exception
    {
        /// <summary>
        /// Initializes new instance of <see cref="UserNotFoundException" /> class.
        /// </summary>
        /// <param name="username">The user</param>
        public UserNotFoundException(string username)
            : base($"User {username} not found!")
        {
        }
    }
}