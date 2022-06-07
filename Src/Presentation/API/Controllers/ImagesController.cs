using Application.Common.Interfaces.Identity;
using Application.Images.Commands.AddImage;
using Application.Images.Commands.DeleteImage;
using Application.Images.Commands.UpdateImage;
using Application.Images.Queries.GetFile;
using Application.Images.Queries.GetUserFiles;
using Domain.Enums.Image;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    /// <summary>
    /// Images controller
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ImagesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;

        /// <summary>
        /// Initializes new instance of <see cref="ImagesController" /> class.
        /// </summary>
        /// <param name="mediator"><see cref="IMediator"/></param>
        /// <param name="currentUserService"><see cref="ICurrentUserService"/></param>
        public ImagesController(IMediator mediator, ICurrentUserService currentUserService)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
        }

        /// <summary>
        /// Gets all users images
        /// </summary>
        /// <returns>User images response list</returns>
        [HttpGet]
        public async Task<IActionResult> GetUserImages()
        {
            var files = await _mediator.Send(new GetUserFilesQuery
            {
                UserId = _currentUserService.UserId
            });

            return Ok(files);
        }
        
        /// <summary>
        /// Gets Image
        /// </summary>
        /// <param name="filename">image name</param>
        /// <param name="id">image version</param>
        /// <returns>image</returns>
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

        /// <summary>
        /// Edits image
        /// </summary>
        /// <param name="file">image name</param>
        /// <param name="width">image width</param>
        /// <param name="height">image height</param>
        /// <param name="targetType">image extension</param>
        /// <param name="version">image version to be edited</param>
        /// <returns>Ok</returns>
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

        /// <summary>
        /// Post image to blob
        /// </summary>
        /// <param name="file">image</param>
        /// <param name="targetType">target extension</param>
        /// <param name="height">target height</param>
        /// <param name="width">target width</param>
        /// <returns>image name</returns>
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

        /// <summary>
        /// Deletes file and/or miniatures with/without specific size
        /// </summary>
        /// <param name="file">filename</param>
        /// <param name="deleteMiniatures">true/false</param>
        /// <param name="size">image sizes to delete</param>
        /// <returns>Ok</returns>
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
