using Application.Common.Interfaces.Database;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.System.Commands.SeedSampleData
{
    public class SampleDataSeeder
    {
        private readonly IJAAADbContext _dbContext;

        public SampleDataSeeder(IJAAADbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task SeedDataAsync(CancellationToken cancellationToken)
        {
            await _dbContext.Users.AddAsync(new User
            {
                Username = "Default",
                Password = "12345"
            }, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
