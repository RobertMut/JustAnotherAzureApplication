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
        public ImagesController(IMediator mediator)
        {
            _mediator = mediator;
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
        public async Task<IActionResult> PostImageAsync([FromForm] IFormFile file, 
            [FromForm] string? targetType,
            [FromForm] int? height,
            [FromForm] int? width)
        {
            await _mediator.Send(new AddImageCommand
            {
                File = file,
                FileName = file.FileName,
                ContentType = file.ContentType,
                TargetType = targetType,
                Height = height,
                Width = width
            });
            return Ok();
        }
    }
}
