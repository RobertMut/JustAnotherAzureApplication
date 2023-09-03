namespace Domain.Entities;

public class GroupUser
{
    /// <summary>
    /// User Guid
    /// </summary>
    public Guid UserId { get; set; }
    /// <summary>
    /// Group Guid
    /// </summary>
    public Guid GroupId { get; set; }
    /// <summary>
    /// Group
    /// </summary>
    public Group Group { get; set; }
    /// <summary>
    /// User
    /// </summary>
    public User User { get; set; }
}