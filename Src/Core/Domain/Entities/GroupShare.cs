namespace Domain.Entities
{
    /// <summary>
    /// Class GroupShare
    /// </summary>
    public class GroupShare
    {
        /// <summary>
        /// Permission id
        /// </summary>
        public int PermissionId { get; set; }
        /// <summary>
        /// Group guid
        /// </summary>
        public Guid GroupId { get; set; }
        /// <summary>
        /// Shared filename
        /// </summary>
        public string Filename { get; set; }
        /// <summary>
        /// Permission
        /// </summary>
        public Permission Permission { get; set; }
        /// <summary>
        /// Group
        /// </summary>
        public Group Group { get; set; }
        /// <summary>
        /// File
        /// </summary>
        public File File { get; set; }
    }
}
