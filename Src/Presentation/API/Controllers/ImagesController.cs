using Application.Images.Commands.AddImage;
using Application.Images.Commands.DeleteImage;
using Application.Images.Commands.UpdateImage;
using Application.Images.Queries.GetFile;
using Domain.Enums.Image;
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

        [HttpGet("{filename}/{id?}")]
        public async Task<IActionResult> GetImageAsync([FromRoute] string filename, int? id)
        {
            var file = await _mediator.Send(new GetFileQuery
            {
                Filename = filename,
                Id = id
            });

            return new FileContentResult(file.File.Content.ToArray(), file.File.Details.ContentType);
        }

        [HttpPut]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateExistingMiniature([FromForm] string file,
            [FromForm] int width,
            [FromForm] int height,
            [FromForm] Format targetType,
            [FromForm] int? version)
        {
            await _mediator.Send(new UpdateImageCommand
            {
                Filename = file,
                Width = width,
                Height = height,
                TargetType = targetType,
                Version = version
            });

            return Ok();
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> PostImageAsync([FromForm] IFormFile file,
            [FromForm] Format targetType,
            [FromForm] int height,
            [FromForm] int width)
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
        
        [HttpDelete]
        [Route("{file}")]
        [Route("{file}/{deleteMiniatures:bool?}")]
        [Route("{file}/{deleteMiniatures:bool?}/{size?}")]
        public async Task<IActionResult> DeleteImageAsync([FromRoute] string file,
            [FromRoute] bool? deleteMiniatures,
            [FromRoute] string? size)
        {
            await _mediator.Send(new DeleteImageCommand
            {
                Filename = file,
                Size=size,
                DeleteMiniatures = deleteMiniatures
            });

            return Ok();
        }
    }
}
