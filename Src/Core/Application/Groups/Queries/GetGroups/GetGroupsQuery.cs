using Application.Common.Interfaces.Database;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using MediatR;

namespace Application.Groups.Queries.GetGroups
{
    /// <summary>
    /// Class GetGroupsQuery
    /// </summary>
    public class GetGroupsQuery : IRequest<GroupListVm>
    {
    }

    /// <summary>
    /// Class GetGroupsQueryHandler
    /// </summary>
    public class GetGroupsQueryHandler : IRequestHandler<GetGroupsQuery, GroupListVm>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes new instance of <see cref="GetGroupsQueryHandler" /> class.
        /// </summary>
        /// <param name="unitOfWork">The <see cref="IUnitOfWork"/></param>
        /// <param name="mapper">The <see cref="IMapper"/></param>
        public GetGroupsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Get Groups
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken">
        /// <see cref="CancellationToken"/>
        /// </param>
        /// <returns>Group list</returns>
        public async Task<GroupListVm> Handle(GetGroupsQuery request, CancellationToken cancellationToken)
        {
            var groups = await _unitOfWork.GroupRepository.GetAsync(cancellationToken: cancellationToken);
            var groupList = groups.AsQueryable().ProjectTo<GroupDto>(_mapper.ConfigurationProvider);

            return new GroupListVm
            {
                Groups = groupList.ToList()
            };
        }
    }
}
