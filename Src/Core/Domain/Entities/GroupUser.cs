namespace Domain.Entities
{
    public class GroupUser
    {
        public Guid UserId { get; set; }
        public Guid GroupId { get; set; }
        public Group Group { get; set; }
        public User User { get; set; }
    }
}
