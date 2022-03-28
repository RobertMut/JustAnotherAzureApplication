using Application.Common.Interfaces.Database;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;

namespace Application.GroupShares.Queries.GetSharesByGroup
{
    public class GetSharesByGroupQuery : IRequest<GroupSharesListVm>
    {
        public string GroupId { get; set; }
    }

    public class GetSharesByGroupQueryHandler : IRequestHandler<GetSharesByGroupQuery, GroupSharesListVm>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetSharesByGroupQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<GroupSharesListVm> Handle(GetSharesByGroupQuery request, CancellationToken cancellationToken)
        {
            var groupShares = await _unitOfWork.GroupShareRepository.GetAsync(x => x.GroupId == Guid.Parse(request.GroupId), cancellationToken: cancellationToken);
            var groupSharesList = groupShares.AsQueryable().ProjectTo<GroupSharesDto>(_mapper.ConfigurationProvider);

            return new GroupSharesListVm
            {
                Shares = groupSharesList.ToList()
            };
        }
    }
}
