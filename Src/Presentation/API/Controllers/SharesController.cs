using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SharesController : ControllerBase
    {
        [HttpGet("/Users/{userId}")]
        public async Task<IActionResult> GetUserShares([FromRoute] string userId)
        {
            throw new NotImplementedException();
        }
        
        [HttpGet("/Groups/{groupId}")]
        public async Task<IActionResult> GetGroupShares([FromRoute] string groupId)
        {
            throw new NotImplementedException();
        }

        [HttpPost("/Users")]
        public async Task<IActionResult> AddUserShare()
        {
            throw new NotImplementedException();
        }

        [HttpPost("/Groups")]
        public async Task<IActionResult> AddGroupShare()
        {
            throw new NotImplementedException();
        }

        [HttpDelete("/Users/{filename}/{userId}")]
        public async Task<IActionResult> DeleteUserShare([FromRoute] string filename,
            [FromRoute] string userId)
        {
            throw new NotImplementedException();
        }

        [HttpDelete("/Groups/{filename}/{groupId}")]
        public async Task<IActionResult> DeleteGroupShare([FromRoute] string filename,
            [FromRoute] string groupId)
        {
            throw new NotImplementedException();
        }
    }
}
