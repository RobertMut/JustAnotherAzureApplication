using Application.Common.Interfaces.Blob;
using Application.Common.Interfaces.Database;
using Domain.Constants.Image;
using Domain.Entities;
using Domain.Enums.Image;
using MediatR;

namespace Application.Images.Queries.GetUserFiles
{
    /// <summary>
    /// Class GetUserFilesQuery
    /// </summary>
    public class GetUserFilesQuery : IRequest<UserFilesListVm>
    {
        /// <summary>
        /// UserId
        /// </summary>
        public string UserId { get; set; }
    }

    /// <summary>
    /// Class GetUserFilesQueryHandler
    /// </summary>
    public class GetUserFilesQueryHandler : IRequestHandler<GetUserFilesQuery, UserFilesListVm>
    {
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Initializes new instance of <see cref="GetUserFilesQueryHandler" /> class.
        /// </summary>
        /// <param name="unitOfWork">The <see cref="IUnitOfWork"/></param>
        public GetUserFilesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Gets user files
        /// </summary>
        /// <param name="request">UserId</param>
        /// <param name="cancellationToken">
        /// <see cref="CancellationToken"/>
        /// </param>
        /// <returns>User files list</returns>
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

