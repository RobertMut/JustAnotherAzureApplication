using Application.Common.Interfaces.Database;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;

namespace Application.GroupShares.Queries.GetSharesByGroup
{
    /// <summary>
    /// Class GetSharesByGroupQuery
    /// </summary>
    public class GetSharesByGroupQuery : IRequest<GroupSharesListVm>
    {
        /// <summary>
        /// GroupId
        /// </summary>
        public string GroupId { get; set; }
    }

    /// <summary>
    /// Class GetSharesByGroupQueryHandler
    /// </summary>
    public class GetSharesByGroupQueryHandler : IRequestHandler<GetSharesByGroupQuery, GroupSharesListVm>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes new instance of <see cref="GetSharesByGroupQueryHandler" /> class.
        /// </summary>
        /// <param name="unitOfWork">The <see cref="IUnitOfWork"/></param>
        /// <param name="mapper">The <see cref="IMapper"/></param>
        public GetSharesByGroupQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets group shares as list
        /// </summary>
        /// <param name="request">GroupId</param>
        /// <param name="cancellationToken">
        /// <see cref="CancellationToken"/>
        /// </param>
        /// <returns>Group shares list</returns>
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
