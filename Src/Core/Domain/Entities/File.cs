namespace Domain.Entities
{
    /// <summary>
    /// Class File
    /// </summary>
    public class File
    {
        /// <summary>
        /// Filename
        /// </summary>
        public string Filename { get; set; }
        /// <summary>
        /// Original filename passed by user
        /// </summary>
        public string OriginalName { get; set; }
        /// <summary>
        /// User Guid
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// User
        /// </summary>
        public User User { get; set; }
        /// <summary>
        /// Group Shares
        /// </summary>
        public ICollection<GroupShare> GroupShares { get; set; }
        /// <summary>
        /// User Shares
        /// </summary>
        public ICollection<UserShare> UserShares { get; set; }
    }
}
