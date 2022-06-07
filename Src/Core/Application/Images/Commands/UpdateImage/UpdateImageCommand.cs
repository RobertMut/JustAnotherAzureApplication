using Application.Common.Helpers.Exception;
using Application.Common.Interfaces.Blob;
using Domain.Common.Helper.Enum;
using Domain.Common.Helper.Filename;
using Domain.Constants.Image;
using Domain.Enums.Image;
using MediatR;
using System.Net;

namespace Application.Images.Commands.UpdateImage
{
    /// <summary>
    /// Class UpdateImageCommand
    /// </summary>
    public class UpdateImageCommand : IRequest
    {
        public string Filename { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Format TargetType { get; set; }
        public int? Version { get; set; }
        public string UserId { get; set; }

        /// <summary>
        /// Class UpdateImageCommandHandler
        /// </summary>
        public class UpdateImageCommandHandler : IRequestHandler<UpdateImageCommand>
        {
            private readonly IBlobManagerService _blobManagerService;

            /// <summary>
            /// Initializes new instance of <see cref="UpdateImageCommandHandler" /> class.
            /// </summary>
            /// <param name="blobManagerService">The blob manager service</param>
            public UpdateImageCommandHandler(IBlobManagerService blobManagerService)
            {
                _blobManagerService = blobManagerService;
            }

            /// <summary>
            /// Updates Image
            /// </summary>
            /// <param name="request">
            /// <see cref="UpdateImageCommand"/>
            /// </param>
            /// <param name="cancellationToken">
            /// <see cref="CancellationToken"/>
            /// </param>
            /// <returns>Unit</returns>
            public async Task<Unit> Handle(UpdateImageCommand request, CancellationToken cancellationToken)
            {
                
                var metadata = new Dictionary<string, string>
                {
                    { Metadata.TargetType, EnumHelper.GetDescriptionFromEnumValue(request.TargetType) },
                    { Metadata.TargetWidth, request.Width.ToString() },
                    { Metadata.TargetHeight, request.Height.ToString() },
                    { Metadata.OriginalFile, request.Filename }
                };

                request.Filename = NameHelper.GenerateHashedFilename(request.Filename);
                string filename = NameHelper.GenerateOriginal(request.UserId, request.Filename);

                if (request.Version != null)
                {
                    var statusCode = await _blobManagerService.PromoteBlobVersionAsync(filename, request.Version.Value, cancellationToken);

                    StatusCode.Check(HttpStatusCode.Created, statusCode, this);
                }

                var updateStatusCode = await _blobManagerService.UpdateAsync(filename, metadata, cancellationToken);
                StatusCode.Check(HttpStatusCode.OK, updateStatusCode, this);

                return Unit.Value;
            }
        }
    }
}
