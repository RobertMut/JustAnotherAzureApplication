using Application.Groups.Commands.AddGroup;
using Application.Groups.Commands.DeleteGroup;
using Application.Groups.Queries.GetGroups;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GroupsController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public GroupsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get groups
    /// </summary>
    /// <returns>All groups</returns>
    [HttpGet]
    public async Task<IActionResult> GetGroups()
    {
        var groups = await _mediator.Send(new GetGroupsQuery());

        return Ok(groups);
    }

    /// <summary>
    /// Adds group
    /// </summary>
    /// <param name="command">Name and group description</param>
    /// <returns>Group guid</returns>
    [HttpPost]
    public async Task<IActionResult> AddGroup([FromBody] AddGroupCommand command)
    {
        var group = await _mediator.Send(command);

        return Ok(group);
    }

    /// <summary>
    /// Deletes group
    /// </summary>
    /// <param name="groupId">Group guid to delete</param>
    /// <returns>Ok</returns>
    [HttpDelete("{groupId:guid}")]
    public async Task<IActionResult> DeleteGroup(Guid groupId)
    {
        await _mediator.Send(new DeleteGroupCommand
        {
            GroupId = groupId
        });

        return Ok();
    }
}