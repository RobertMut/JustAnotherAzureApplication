namespace Domain.Entities;

public class Group
{
    /// <summary>
    /// Group guid
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// Group Name
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// Group Description
    /// </summary>
    public string? Description { get; set; }
    /// <summary>
    /// Group Users
    /// </summary>
    public ICollection<GroupUser> GroupUsers { get; set; }
    /// <summary>
    /// Group shares
    /// </summary>
    public ICollection<GroupShare> GroupShares { get; set; }
}