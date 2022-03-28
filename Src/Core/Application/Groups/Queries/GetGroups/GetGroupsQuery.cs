using Application.Common.Interfaces.Database;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using MediatR;

namespace Application.Groups.Queries.GetGroups
{
    public class GetGroupsQuery : IRequest<GroupListVm>
    {
    }

    public class GetGroupsQueryHandler : IRequestHandler<GetGroupsQuery, GroupListVm>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetGroupsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

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
