using Application.Common.Helpers.Exception;
using Application.Common.Interfaces.Blob;
using Application.Common.Interfaces.Database;
using Common;
using Domain.Common.Helper.Enum;
using Domain.Common.Helper.Filename;
using Domain.Constants.Image;
using Domain.Enums.Image;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using File = Domain.Entities.File;

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
            private readonly IUnitOfWork _unitOfWork;
            private readonly IBlobManagerService _service;

            public AddImageCommandHandler(IBlobManagerService service, IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
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

                string originalFilenameMD5 = NameHelper.GenerateHashedFilename(request.Filename);
                string filename = NameHelper.GenerateOriginal(request.UserId, originalFilenameMD5);

                using (var stream = request.File.OpenReadStream())
                {
                    var statusCode = await _service.AddAsync(stream, filename, request.ContentType, metadata, cancellationToken);

                    StatusCode.Check(HttpStatusCode.Created, statusCode, this);
                }

                var fileEntry = await _unitOfWork.FileRepository.GetObjectBy(x => x.Filename == filename, cancellationToken: cancellationToken);
                if (fileEntry == null)
                {
                    await _unitOfWork.FileRepository.InsertAsync(new File
                    {
                        Filename = filename,
                        OriginalName = request.Filename,
                        UserId = Guid.Parse(request.UserId)
                    }, cancellationToken: cancellationToken);
                    await _unitOfWork.Save(cancellationToken);
                }

                return request.Filename;
            }
        }
    }
}
