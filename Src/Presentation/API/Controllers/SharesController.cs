using Application.GroupShares.Commands.AddGroupShare;
using Application.GroupShares.Commands.DeleteGroupShare;
using Application.GroupShares.Queries.GetSharesByGroup;
using Application.UserShares.Commands.AddUserShare;
using Application.UserShares.Commands.DeleteUserShare;
using Application.UserShares.Queries.GetSharesByUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SharesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SharesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("/Users/{userId}")]
        public async Task<IActionResult> GetUserShares([FromRoute] string userId)
        {
            var shares = await _mediator.Send(new GetSharesByUserQuery
            {
                UserId = userId
            }, new CancellationToken());

            return Ok(shares);
        }
        
        [HttpGet("/Groups/{groupId}")]
        public async Task<IActionResult> GetGroupShares([FromRoute] string groupId)
        {
            var shares = await _mediator.Send(new GetSharesByGroupQuery
            {
                GroupId = groupId
            }, new CancellationToken());

            return Ok(shares);  
        }

        [HttpPost("/Users")]
        public async Task<IActionResult> AddUserShare([FromBody] AddUserShareCommand command)
        {
            return Ok(await _mediator.Send(command, new CancellationToken()));
        }

        [HttpPost("/Groups")]
        public async Task<IActionResult> AddGroupShare([FromBody] AddGroupShareCommand command)
        {
            return Ok(await _mediator.Send(command, new CancellationToken()));
        }

        [HttpDelete("/Users/{filename}/{userId}")]
        public async Task<IActionResult> DeleteUserShare([FromRoute] string filename,
            [FromRoute] string userId)
        {
            await _mediator.Send(new DeleteUserShareCommand
            {
                Filename = filename,
                UserId = userId
            }, new CancellationToken());

            return Ok();
        }

        [HttpDelete("/Groups/{filename}/{groupId}")]
        public async Task<IActionResult> DeleteGroupShare([FromRoute] string filename,
            [FromRoute] string groupId)
        {
            await _mediator.Send(new DeleteGroupShareCommand
            {
                Filename = filename,
                GroupId = groupId
            }, new CancellationToken());

            return Ok();
        }
    }
}
