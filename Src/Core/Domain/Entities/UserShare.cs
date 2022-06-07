namespace Domain.Entities
{
    /// <summary>
    /// Class UserShare
    /// </summary>
    public class UserShare
    {
        /// <summary>
        /// Permission id
        /// </summary>
        public int PermissionId { get; set; }
        /// <summary>
        /// User guid
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// File to be shared
        /// </summary>
        public string Filename { get; set; }
        /// <summary>
        /// User
        /// </summary>
        public User User { get; set; }
        /// <summary>
        /// File
        /// </summary>
        public File File { get; set; }
        /// <summary>
        /// Permission
        /// </summary>
        public Permission Permission { get; set; }
    }
}
