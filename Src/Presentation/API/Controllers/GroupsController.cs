using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class GroupsController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetGroups()
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public async Task<IActionResult> AddGroup()
        {
            throw new NotImplementedException();
        }

        [HttpDelete("groupId")]
        public async Task<IActionResult> DeleteGroup([FromRoute] string groupId)
        {
            throw new NotImplementedException();
        }
    }
}
