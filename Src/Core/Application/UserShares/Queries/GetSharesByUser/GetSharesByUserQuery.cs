using Application.Common.Interfaces.Database;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using MediatR;

namespace Application.UserShares.Queries.GetSharesByUser;

public class GetSharesByUserQuery : IRequest<UserSharesListVm>
{
    /// <summary>
    /// UserId
    /// </summary>
    public Guid UserId { get; set; }
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

    /// <summary>
    /// Get all shares by user id
    /// </summary>
    /// <param name="request">User Id</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns>User shares list</returns>
    public async Task<UserSharesListVm> Handle(GetSharesByUserQuery request, CancellationToken cancellationToken)
    {
        var userShares = await _unitOfWork.UserShareRepository.GetAsync(x => x.UserId == request.UserId, cancellationToken: cancellationToken);
        var userSharesList = userShares.AsQueryable().ProjectTo<UserSharesDto>(_mapper.ConfigurationProvider);

        return new UserSharesListVm
        {
            Shares = userSharesList.ToList()
        };
    }
}