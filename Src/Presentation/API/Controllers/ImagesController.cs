using Application.Images.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImagesController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ImagesController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet]
        public async Task<IActionResult> GetImageAsync()
        {
            throw new NotImplementedException();
        }
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> PostImageAsync(IFormFile file)
        {
            await _mediator.Send(new AddImageCommand
            {
                File = file,
                FileName = file.FileName,
                ContentType = file.ContentType
            });
            return Ok();
        }
    }
}
