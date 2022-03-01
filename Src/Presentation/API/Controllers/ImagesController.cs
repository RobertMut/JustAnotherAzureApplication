using Application.Images.Commands;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Web.Http;

namespace API.Controllers
{
    [Route("api/[controller]")]
    public class ImagesController : ApiController
    {
        private readonly IMediator _mediator;
        public ImagesController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet]
        public async Task<IHttpActionResult> GetImageAsync()
        {
            throw new NotImplementedException();
        }
        [HttpPost]
        public async Task<IHttpActionResult> PostImageAsync(IFormFile file)
        {
            await _mediator.Send(new AddImageCommand
            {
                File = file
            });
            return Ok();
        }
    }
}
