using Application.Common.Interfaces.Database;
using MediatR;

namespace Application.System.Commands.SeedSampleData
{
    public class SeedSampleDataCommand : IRequest
    {
        public class SeedSampleDataCommandHandler : IRequestHandler<SeedSampleDataCommand>
        {
            public readonly IJAAADbContext _dbContext;

            public SeedSampleDataCommandHandler(IJAAADbContext dbContext)
            {
                _dbContext = dbContext;
            }

            public async Task<Unit> Handle(SeedSampleDataCommand request, CancellationToken cancellationToken)
            {
                var seeder = new SampleDataSeeder(_dbContext);

                await seeder.SeedDataAsync(cancellationToken);

                return Unit.Value;
            }
        }
    }
}
