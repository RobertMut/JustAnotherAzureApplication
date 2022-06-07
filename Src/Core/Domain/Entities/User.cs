namespace Domain.Entities
{
    /// <summary>
    /// Class User
    /// </summary>
    public class User
    {
        /// <summary>
        /// User guid
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Username
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// Files
        /// </summary>
        public ICollection<File> Files { get; set; }
        /// <summary>
        /// Group Users
        /// </summary>
        public ICollection<GroupUser> GroupUsers { get; set; }
        /// <summary>
        /// User Shares
        /// </summary>
        public ICollection<UserShare> UserShares { get; set; }
    }
}
