namespace Domain.Entities
{
    public class Group
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public ICollection<GroupUser> GroupUsers { get; set; }
        public ICollection<GroupShare> GroupShares { get; set; }
    }
}
