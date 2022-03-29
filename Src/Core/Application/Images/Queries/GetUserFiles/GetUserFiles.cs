using Application.Common.Interfaces.Blob;
using Application.Common.Interfaces.Database;
using Domain.Constants.Image;
using Domain.Entities;
using Domain.Enums.Image;
using MediatR;

namespace Application.Images.Queries.GetUserFiles
{
    public class GetUserFilesQuery : IRequest<UserFilesListVm>
    {
        public string UserId { get; set; }
    }

    public class GetUserFilesQueryHandler : IRequestHandler<GetUserFilesQuery, UserFilesListVm>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetUserFilesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<UserFilesListVm> Handle(GetUserFilesQuery request, CancellationToken cancellationToken)
        {
            var userFiles = await _unitOfWork.FileRepository.GetAsync(x => x.UserId == Guid.Parse(request.UserId), cancellationToken: cancellationToken);
            var userGroups = await _unitOfWork.GroupUserRepository.GetAsync(x => x.UserId == Guid.Parse(request.UserId));
            List<GroupShare> groupShares = new List<GroupShare>();

            foreach (var group in userGroups)
            {
                groupShares.Add(await _unitOfWork.GroupShareRepository.GetObjectBy(x => group.GroupId == x.GroupId));
            }

            var files = (from userFile in userFiles
                         select new FileDto
                         {
                             Filename = userFile.Filename,
                             IsOwned = true,
                             Permission = null,
                             OriginalName = userFile.OriginalName,
                         }).Concat(from userShared in await _unitOfWork.UserShareRepository.GetAsync(x => x.UserId == Guid.Parse(request.UserId))
                                   select new FileDto
                                   {
                                       Filename = userShared.Filename,
                                       IsOwned = false,
                                       OriginalName = null,
                                       Permission = (Permissions)userShared.PermissionId
                                   }).Concat(from groupShared in groupShares
                                             select new FileDto
                                             {
                                                 Filename = groupShared.Filename,
                                                 IsOwned = false,
                                                 OriginalName = null,
                                                 Permission = (Permissions)groupShared.PermissionId
                                             });

            return new UserFilesListVm()
            {
                Files = files
            };
        }
    }
}

