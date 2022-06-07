namespace Application.Common.Exceptions
{
    /// <summary>
    /// Class ShareNotFoundException
    /// </summary>
    public class ShareNotFoundException : Exception
    {
        /// <summary>
        /// Initializes new instance of <see cref="ShareNotFoundException" /> class.
        /// </summary>
        /// <param name="shareName">The name of share</param>
        /// <param name="id">The user id or group id</param>
        public ShareNotFoundException(string shareName, string id)
            : base($"Share {shareName} was not found for id {id}")
        {
        }
    }
}
