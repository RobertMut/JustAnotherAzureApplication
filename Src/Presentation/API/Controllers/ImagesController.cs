using Application.Images.Commands;
using Application.Images.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImagesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;
        public ImagesController(IMediator mediator, IConfiguration configuration)
        {
            _mediator = mediator;
            _configuration = configuration;
        }
        [HttpGet("{filename}")]
        public async Task<IActionResult> GetImageAsync([FromRoute]string filename)
        {
            var file = await _mediator.Send(new GetFileQuery
            {
                Filename = filename
            });
            return new FileContentResult(file.File.Content.ToArray(), file.File.Details.ContentType);
        }
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> PostImageAsync([FromForm] IFormFile file)
        {
            await _mediator.Send(new AddImageCommand
            {
                File = file,
                FileName = file.FileName,
                ContentType = file.ContentType,

            });
            return Ok();
        }
    }
}
