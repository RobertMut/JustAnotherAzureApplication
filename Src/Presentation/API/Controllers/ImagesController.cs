using Application.Common.Interfaces.Identity;
using Application.Images.Commands.AddImage;
using Application.Images.Commands.DeleteImage;
using Application.Images.Commands.UpdateImage;
using Application.Images.Queries.GetFile;
using Domain.Enums.Image;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ImagesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;

        public ImagesController(IMediator mediator, ICurrentUserService currentUserService)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
        }

        [HttpGet("{filename}/{id?}")]
        public async Task<IActionResult> GetImageAsync([FromRoute] string filename, int? id)
        {
            var file = await _mediator.Send(new GetFileQuery
            {
                Filename = filename,
                Id = id,
                UserId = _currentUserService.UserId,
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
                Version = version,
                UserId = _currentUserService.UserId
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
            string filename = await _mediator.Send(new AddImageCommand
            {
                File = file,
                Filename = file.FileName,
                ContentType = file.ContentType,
                TargetType = targetType,
                Height = height,
                Width = width,
                UserId = _currentUserService.UserId
            });

            return Ok(filename);
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
                Size = size,
                DeleteMiniatures = deleteMiniatures,
                UserId = _currentUserService.UserId
            });

            return Ok();
        }
    }
}
