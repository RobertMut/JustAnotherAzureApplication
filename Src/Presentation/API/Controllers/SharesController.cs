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
    /// <summary>
    /// Class SharesController
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SharesController : ControllerBase
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Initializes new instance of <see cref="SharesController" /> class.
        /// </summary>
        /// <param name="mediator"><see cref="IMediator"/></param>
        public SharesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get user shares by id
        /// </summary>
        /// <param name="userId">user id</param>
        /// <returns>shares</returns>
        [HttpGet("/Users/{userId}")]
        public async Task<IActionResult> GetUserShares([FromRoute] string userId)
        {
            var shares = await _mediator.Send(new GetSharesByUserQuery
            {
                UserId = userId
            }, new CancellationToken());

            return Ok(shares);
        }
        
        /// <summary>
        /// Get group shares by group id
        /// </summary>
        /// <param name="groupId">group id</param>
        /// <returns>shares</returns>
        [HttpGet("/Groups/{groupId}")]
        public async Task<IActionResult> GetGroupShares([FromRoute] string groupId)
        {
            var shares = await _mediator.Send(new GetSharesByGroupQuery
            {
                GroupId = groupId
            }, new CancellationToken());

            return Ok(shares);  
        }

        /// <summary>
        /// Adds user share
        /// </summary>
        /// <param name="command">user id who shares,
        /// other user id who get share,
        /// filename to share,
        /// permissions to file
        /// </param>
        /// <returns>user id</returns>
        [HttpPost("/Users")]
        public async Task<IActionResult> AddUserShare([FromBody] AddUserShareCommand command)
        {
            return Ok(await _mediator.Send(command, new CancellationToken()));
        }

        /// <summary>
        /// Adds group share
        /// </summary>
        /// <param name="command">user id who shares,
        /// group id who get share,
        /// filename to share,
        /// permissions to file
        /// </param>
        /// <returns>user id</returns>
        [HttpPost("/Groups")]
        public async Task<IActionResult> AddGroupShare([FromBody] AddGroupShareCommand command)
        {
            return Ok(await _mediator.Send(command, new CancellationToken()));
        }

        /// <summary>
        /// Delete user share
        /// </summary>
        /// <param name="filename">image name</param>
        /// <param name="userId">user id share</param>
        /// <returns>Ok</returns>
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

        /// <summary>
        /// Delete group share
        /// </summary>
        /// <param name="filename">image name</param>
        /// <param name="groupId">group id</param>
        /// <returns>Ok</returns>
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
