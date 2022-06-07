namespace Application.Common.Exceptions
{
    /// <summary>
    /// Class AccessDeniedException
    /// </summary>
    public class AccessDeniedException : Exception
    {
        /// <summary>
        /// Initializes new instance of <see cref="AccessDeniedException" /> class.
        /// </summary>
        /// <param name="user">The user without access</param>
        /// <param name="obj">The object he tried to access to</param>
        public AccessDeniedException(string user, string obj) : 
            base($"Access denied for {user} to {obj}")
        {
        }
    }
}
