namespace Domain.Entities
{
    public class UserShare
    {
        public int PermissionId { get; set; }
        public Guid UserId { get; set; }
        public string Filename { get; set; }
        public User User { get; set; }
        public File File { get; set; }
        public Permission Permission { get; set; }
    }
}
