using Application.Groups.Commands.AddGroup;
using Application.Groups.Commands.DeleteGroup;
using Application.Groups.Queries.GetGroups;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
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

        [HttpGet]
        public async Task<IActionResult> GetGroups()
        {
            var groups = await _mediator.Send(new GetGroupsQuery(), new CancellationToken());

            return Ok(groups);
        }

        [HttpPost]
        public async Task<IActionResult> AddGroup([FromBody] AddGroupCommand command)
        {
            var group = await _mediator.Send(command, new CancellationToken());

            return Ok(group);
        }

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
