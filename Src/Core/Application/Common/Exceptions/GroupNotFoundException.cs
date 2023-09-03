namespace Application.Common.Exceptions;

public class GroupNotFoundException : Exception
{
    public GroupNotFoundException(string groupName)
        : base($"Group named {groupName} not found!")
    {
    }
    
    public GroupNotFoundException(Guid id)
        : base($"Group with {id.ToString()} not found!")
    {
    }
}