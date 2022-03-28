namespace Domain.Entities
{
    public class GroupShare
    {
        public int PermissionId { get; set; }
        public Guid GroupId { get; set; }
        public string Filename { get; set; }
        public Permission Permission { get; set; }
        public Group Group { get; set; }
        public File File { get; set; }
    }
}
