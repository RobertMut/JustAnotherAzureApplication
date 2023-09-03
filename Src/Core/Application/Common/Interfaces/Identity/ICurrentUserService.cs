namespace Application.Common.Interfaces.Identity;

/// <summary>
/// Determines ICurrentUserService interface with UserId string.
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Current user id
    /// </summary>
    string UserId { get; }
}