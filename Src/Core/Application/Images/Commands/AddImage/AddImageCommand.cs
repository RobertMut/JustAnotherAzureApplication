using Application.Common.Helpers.Exception;
using Application.Common.Interfaces.Blob;
using Common;
using Domain.Common.Helper.Enum;
using Domain.Common.Helper.Filename;
using Domain.Constants.Image;
using Domain.Enums.Image;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace Application.Images.Commands.AddImage
{
    public class AddImageCommand : IRequest<string>
    {
        public IFormFile File { get; set; }
        public string Filename { get; set; }
        public string ContentType { get; set; }
        public Format? TargetType { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string UserId { get; set; }

        public class AddImageCommandHandler : IRequestHandler<AddImageCommand, string>
        {
            private readonly IDateTime _dateTime;
            private readonly IBlobManagerService _service;

            public AddImageCommandHandler(IBlobManagerService service, IDateTime dateTime)
            {
                _dateTime = dateTime;
                _service = service;
            }

            public async Task<string> Handle(AddImageCommand request, CancellationToken cancellationToken)
            {

                var metadata = new Dictionary<string, string>
                {
                    { Metadata.OriginalFile, request.Filename },
                    { Metadata.TargetType, !request.TargetType.HasValue ? request.ContentType : EnumHelper.GetDescriptionFromEnumValue(request.TargetType.Value) },
                    { Metadata.TargetWidth, request.Width.ToString() },
                    { Metadata.TargetHeight, request.Height.ToString() },
                };
                request.Filename = request.Filename.Replace(char.Parse(Name.Delimiter), '-');
                var existingBlobs = await _service.GetBlobsInfoByName(Prefixes.OriginalImage, null, $"{request.Filename}", request.UserId, cancellationToken);

                if (existingBlobs.Count() > 0)
                {
                    request.Filename = $"{_dateTime.Now.ToString("yyyyMMddHHmmssffff")}-{request.Filename}";
                }
                
                string filename = NameHelper.GenerateOriginal(request.UserId, request.Filename);

                using (var stream = request.File.OpenReadStream())
                {
                    var statusCode = await _service.AddAsync(stream, filename, request.ContentType, metadata, cancellationToken);

                    StatusCode.Check(HttpStatusCode.Created, statusCode, this);
                }

                return request.Filename;
            }
        }
    }
}
