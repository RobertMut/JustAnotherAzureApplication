using Application.Groups.Commands.AddGroup;
using Application.Groups.Commands.DeleteGroup;
using Application.Groups.Queries.GetGroups;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    /// <summary>
    /// Class GroupsContoller
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class GroupsController : ControllerBase
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Initializes new instance of <see cref="GroupsController" /> class.
        /// </summary>
        /// <param name="mediator"><see cref="IMediator"/></param>
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
            var groups = await _mediator.Send(new GetGroupsQuery(), new CancellationToken());

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
            var group = await _mediator.Send(command, new CancellationToken());

            return Ok(group);
        }

        /// <summary>
        /// Deletes group
        /// </summary>
        /// <param name="groupId">Group guid to delete</param>
        /// <returns>Ok</returns>
        [HttpDelete("groupId")]
        public async Task<IActionResult> DeleteGroup([FromRoute] string groupId)
        {
            await _mediator.Send(new DeleteGroupCommand
            {
                GroupId = groupId
            }, new CancellationToken());

            return Ok();
        }
    }
}
