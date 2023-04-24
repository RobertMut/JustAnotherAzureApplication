using Application.Common.Interfaces.Database;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using MediatR;

namespace Application.UserShares.Queries.GetSharesByUser
{
    /// <summary>
    /// Class GetSharesByUserQuery
    /// </summary>
    public class GetSharesByUserQuery : IRequest<UserSharesListVm>
    {
        /// <summary>
        /// UserId
        /// </summary>
        public string UserId { get; set; }
    }

    /// <summary>
    /// Class GetSharesByUserQueryHandler
    /// </summary>
    public class GetSharesByUserQueryHandler : IRequestHandler<GetSharesByUserQuery, UserSharesListVm>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes new instance of <see cref="GetSharesByUserQueryHandler" /> class.
        /// </summary>
        /// <param name="unitOfWork">The <see cref="IUnitOfWork"/></param>
        /// <param name="mapper">The <see cref="IMapper"/></param>
        public GetSharesByUserQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all shares by user id
        /// </summary>
        /// <param name="request">User Id</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
        /// <returns>User shares list</returns>
        public async Task<UserSharesListVm> Handle(GetSharesByUserQuery request, CancellationToken cancellationToken)
        {
            var userShares = await _unitOfWork.UserShareRepository.GetAsync(x => x.UserId == Guid.Parse(request.UserId), cancellationToken: cancellationToken);
            var userSharesList = userShares.AsQueryable().ProjectTo<UserSharesDto>(_mapper.ConfigurationProvider);

            return new UserSharesListVm
            {
                Shares = userSharesList.ToList()
            };
        }
    }
}
