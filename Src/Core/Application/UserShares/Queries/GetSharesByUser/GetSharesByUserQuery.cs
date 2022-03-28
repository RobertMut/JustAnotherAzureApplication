using Application.Common.Interfaces.Database;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;

namespace Application.UserShares.Queries.GetSharesByUser
{
    public class GetSharesByUserQuery : IRequest<UserSharesListVm>
    {
        public string UserId { get; set; }
    }

    public class GetSharesByUserQueryHandler : IRequestHandler<GetSharesByUserQuery, UserSharesListVm>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetSharesByUserQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

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
